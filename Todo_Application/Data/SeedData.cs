using Microsoft.AspNetCore.Identity;

namespace Todo_Application.Data
{
    public static class SeedData
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            if (!await roleManager.RoleExistsAsync("Manager"))
            {
                await roleManager.CreateAsync(new IdentityRole("Manager"));
            }

            var testManager = await userManager.FindByNameAsync("testmanager@test.com");

            if (testManager == null) {
                testManager = new IdentityUser
                {
                    UserName = "testmanager@test.com",
                    Email = "testmanager@test.com",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync( testManager,"@Abc123");
                if (!result.Succeeded) 
                {
                    throw new Exception("Failed to create testmanager: "+string.Join(", ", result.Errors.Select(e=>e.Description)));
                }
            }

            if(!await userManager.IsInRoleAsync(testManager, "Manager"))
            {
                await userManager.AddToRoleAsync(testManager, "Manager");
            }
        }
    }
}
