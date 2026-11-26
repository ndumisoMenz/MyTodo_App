
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Todo_Application.Models;
using Todo_Application.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class TodoesController : Controller
{
    private readonly ApplicationDbContext _context;

    public TodoesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: TODOS
    public async Task<IActionResult> Index(string searchString,string statusFilter,int pageNumber=1)
    {
        var query = _context.Todos.Include(t => t.User).AsQueryable();
        if (!User.IsInRole("Manager"))
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            query=query.Where(t=>t.UserId == userId);
        }

        if (!string.IsNullOrEmpty(searchString))
        {
            query = query.Where(t => t.Title.Contains(searchString) || (t.User != null && t.User.UserName.Contains(searchString)));
        }

        if (statusFilter == "Completed")
        {
            query=query.Where(t=>t.isComplted==true);
        }
        else if(statusFilter == "NotCompleted")
        {
            query=query.Where(t=>t.isComplted==false);
        }

        int pageSize = 2;
        int totalCount = await query.CountAsync();
        int totalPages=(int)Math.Ceiling((double)totalCount/pageSize);

        var todos = await query.OrderBy(t => t.DueDate).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        ViewBag.Pagination = new PaginationInfo
        {
            CurrentPage = pageNumber,
            TotalPages = totalPages,
            SearchString = searchString,
            StatusFilter = statusFilter,
            ActionName = "Index",
            ControllerName = "Todoes"
        };
        return View(todos);
    }

    // GET: TODOS/Details/5
    [HttpPost]
    public async Task<IActionResult> Toggle(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var todo = await _context.Todos
            .FirstOrDefaultAsync(m => m.Id == id);

        if (todo == null)
        {
            return NotFound();
        }
        todo.isComplted= !todo.isComplted;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: TODOS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: TODOS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft
    // .com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Title,isComplted,Description,DateCreated,DueDate,User")] Todo todo)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        todo.UserId = userId;
        todo.DateCreated = DateTime.UtcNow;
        todo.DueDate=DateTime.SpecifyKind(todo.DueDate, DateTimeKind.Utc);
        ModelState.Remove("User");
        ModelState.Remove("UserId");
        if (ModelState.IsValid)
        {
            _context.Add(todo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(todo);
    }

    // GET: TODOS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var todo = await _context.Todos.FindAsync(id);
        if (todo == null)
        {
            return NotFound();
        }
        return View(todo);
    }

    // POST: TODOS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Title,isComplted,Description,DateCreated,DueDate,User")] Todo todo)
    {
        if (id != todo.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(todo);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoExists(todo.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(todo);
    }

    // GET: TODOS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var todo = await _context.Todos
            .FirstOrDefaultAsync(m => m.Id == id);
        if (todo == null)
        {
            return NotFound();
        }

        return View(todo);
    }

    // POST: TODOS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var todo = await _context.Todos.FindAsync(id);
        if (todo != null)
        {
            _context.Todos.Remove(todo);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool TodoExists(int? id)
    {
        return _context.Todos.Any(e => e.Id == id);
    }
}
