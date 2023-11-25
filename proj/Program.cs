using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using proj.Data;
using static System.Formats.Asn1.AsnWriter;
using System;
using NuGet.Protocol.Core.Types;
using proj.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using proj.Authorization;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
            .AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

        builder.Services.AddControllersWithViews();
        builder.Services.AddScoped<IDatabaseRepository, DatabaseRepository>();
        IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
        builder.Services.AddSingleton(mapper);
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("ViewResumePolicy", policy =>
            {
                policy.Requirements.Add(new ViewResumeRequirement());
            });
        });

        builder.Services.AddScoped<IAuthorizationHandler, ViewResumeAuthorizationHandler>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
        }
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        app.MapRazorPages();
        // to be able to use the services added here
        using (var scope = app.Services.CreateScope())//similar to seeder
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var roles = new[] { "Admin", "Visitor" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        using (var scope = app.Services.CreateScope())//similar to seeder
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            String email = "Admin@admin.com";
            String pass = "123Test!";
            if(await userManager.FindByEmailAsync(email) == null)
            {
                var user = new IdentityUser();
                user.Email = email;
                user.UserName = email;
                user.EmailConfirmed=true;
                await userManager.CreateAsync(user, pass);
                await userManager.AddToRoleAsync(user,"Admin");
            }
        }

        ApplyMigration();
        app.Run();
        void ApplyMigration()
        {
            var scope = app.Services.CreateScope();
            var _db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            if (_db.Database.GetPendingMigrations().Count() > 0)
            {
                _db.Database.Migrate();
            }
        }
    }
}