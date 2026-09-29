using Microsoft.AspNetCore.Identity;

namespace BusReservationERP.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string adminEmail = "admin@busreservation.com";
            string adminPassword = "Admin@123";
            string roleName = "Admin";

            // Step 1: Agar "Admin" role exist nahi karta to bana do
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            // Step 2: Agar admin user exist nahi karta to bana do
            var existingUser = await userManager.FindByEmailAsync(adminEmail);
            if (existingUser == null)
            {
                var adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    // Step 3: User ko "Admin" role assign karo
                    await userManager.AddToRoleAsync(adminUser, roleName);
                }
            }
        }
    }
}