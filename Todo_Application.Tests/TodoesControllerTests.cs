using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Todo_Application.Data;
using Todo_Application.Models;

namespace Todo_Application.Tests
{
    public class TodoesControllerTests
    {
        private (ApplicationDbContext context, TodoesController controller) CreateController()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().
                UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);
            var controller = new TodoesController(context);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user-123"),
                new Claim(ClaimTypes.Role, "User")
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            return (context, controller);
        }

        [Fact]

        public async Task Create_ValidTodo_SavesToDatabase()
        {
            var (context,controller)= CreateController();
            var todo = new Todo {
                Title = "Buy Milk",
                Description = "Buy milk from the shop",
                DateCreated = DateTime.Now,
                DueDate = DateTime.Now.AddDays(1),
                UserId = "user-123" };

            await controller.Create(todo);
            Assert.Single(context.Todos);
            Assert.Equal("Buy Milk", context.Todos.First().Title);
        }

        [Fact]

        public async Task Toggle_Todo_FlipIsCompleted()
        {
            var (context, controller) = CreateController();
            context.Todos.Add(new Todo { Id = 1,
                Title = "Test",
                isComplted = false, 
                Description="Test Description",
                UserId = "user-123" });
            await context.SaveChangesAsync();

            await controller.Toggle(1);

            Assert.True(context.Todos.First().isComplted);
        }

        [Fact]

        public async Task Index_SearchString_FiltersByTitle()
        {
            var (context, controller) = CreateController();
            context.Users.Add(new IdentityUser
            {
                Id = "user-123",
                UserName = "testuser"
            });
            context.Todos.AddRange(
                new Todo 
                {
                    Title = "Buy shopping",
                    Description = "Go shopping",
                    DueDate = DateTime.Now.AddDays(1),
                    isComplted = false,
                    UserId = "user-123"
                },
                new Todo
                {
                    Title = "Study C#",
                    Description = "Study for test",
                    DueDate = DateTime.Now.AddDays(1),
                    isComplted = false,
                    UserId = "user-123"
                });
            await context.SaveChangesAsync();

            var result= await controller.Index("shopping", null,1) as ViewResult;
            var model = result.Model as IEnumerable<Todo>;

            Assert.Single(model);
            Assert.Equal("Buy shopping", model.First().Title);
        }

        [Fact]

        public async Task Index_StatusFilter_FiltersByCompleted()
        {
            var (context, controller) = CreateController();

            context.Users.Add(new IdentityUser
            {
                Id = "user-123",
                UserName = "testuser"
            });

            context.Todos.AddRange(
                new Todo { Title = "Done task", Description = "Done", DueDate = DateTime.Now.AddDays(1), isComplted = true, UserId = "user-123" },
                new Todo
                {
                    Title = "Not done task",
                    Description = "Not done",
                    DueDate = DateTime.Now.AddDays(1),
                    isComplted = false,
                    UserId = "user-123"

                });

                await context.SaveChangesAsync();

                var result = await controller.Index(null, "Completed", 1) as ViewResult;
                var model = result.Model as IEnumerable<Todo>;

                Assert.Single (model);
                Assert.Equal("Done task", model.First().Title);
        }
    }
}

