using Microsoft.EntityFrameworkCore;
using KapdaStore.Data;

namespace KapdaStore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ? Add MVC and Razor Views
            builder.Services.AddControllersWithViews();

            // ? Add Session (timeout configured)
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);  // ?? More practical timeout
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // ? Antiforgery Token setup
            builder.Services.AddAntiforgery(options =>
            {
                options.HeaderName = "RequestVerificationToken"; // ?? Header name for JavaScript
            });

            // ? Add EF Core
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();

            // ? Middleware Configuration
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // ? Session must come BEFORE Authorization
            app.UseSession();

            // ? For AntiForgery to work, this is needed (Authorization is optional but must be placed correctly)
            app.UseAuthentication(); // optional
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Prakash}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
