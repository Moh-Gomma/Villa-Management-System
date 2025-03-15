using Hotel.Application.Common.Interfaces;
using Hotel.Infrastructue.Data;
using Hotel.Infrastructue.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Hotel.Domain.Entities;
using Stripe;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http.Features;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();


        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultUI()
            .AddDefaultTokenProviders();

        // Add Repository
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/AccessDenied";
            options.LogoutPath = "/Account/LogOut";
            options.SlidingExpiration = true;
            options.ReturnUrlParameter = "returnUrl";
        });

        // Configure Logging
        builder.Services.AddLogging(logging =>
        {
            logging.AddConsole();
            logging.AddDebug();
        });

        builder.Services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = 10 * 1024 * 1024; 
        });

        var app = builder.Build();

        // Stripe
        StripeConfiguration.ApiKey = app.Configuration.GetSection("Stripe:SecretKey").Get<string>();


        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles(); 
        app.UseRouting();

        app.UseStatusCodePagesWithRedirects("~/Home/Index"); 

        app.UseAuthentication();
        app.UseAuthorization();

        // Seed default admin
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                await DbInitializer.SeedDefaultAdmin(services);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}