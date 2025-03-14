using Hotel.Application.Common.Interfaces;
using Hotel.Infrastructue.Data;
using Hotel.Infrastructue.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Hotel.Domain.Entities;
using Stripe;

namespace Hotel
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            ///Adding Database 
            builder.Services.AddDbContext<ApplicationDbContext>(options => 
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<ApplicationUser , IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultUI();

            //Adding Repo
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.LogoutPath = "/Account/LogOut";
                options.SlidingExpiration = true;
                options.ReturnUrlParameter = "returnUrl";
            });

            var app = builder.Build();

            //Stripe
            StripeConfiguration.ApiKey = app.Configuration.GetSection("Stripe:SecretKey").Get<string>();


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseStatusCodePagesWithRedirects("~/Home/Index"); // Redirects 404 to Home page

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();
  

            app.Run();
        }
    }
}
