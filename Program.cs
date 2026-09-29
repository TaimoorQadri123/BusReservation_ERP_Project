using Microsoft.EntityFrameworkCore;
using BusReservationERP.Data;
using Microsoft.AspNetCore.Identity;

namespace BusReservationERP

{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();


            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = false; // practice ke liye simple rakha
                options.Password.RequiredLength = 6;
            })
              .AddEntityFrameworkStores<AppDbContext>()
              .AddDefaultTokenProviders();

            var app = builder.Build();

            // App start hote hi Admin account seed karo (agar already nahi hai)
            using (var scope = app.Services.CreateScope())
            {
                await BusReservationERP.Data.DbSeeder.SeedAdminUserAsync(scope.ServiceProvider);
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

          

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            await app.RunAsync();
        }
    }
}
