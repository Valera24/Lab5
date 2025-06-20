using Lab3.Data;
using Lab4.Areas.Identity.Data;
using Lab4.Data;
using Lab5;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<RosterContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("RosterContext")));
builder.Services.AddDbContext<Lab4Context>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("RosterContext")));
builder.Services.AddIdentity<Lab4User, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<Lab4Context>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    options.SignIn.RequireConfirmedAccount = false;
    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
        options.SaveTokens = true;
        options.CallbackPath = "/signin-google";

        // Запрос дополнительных данных
        options.Scope.Add("profile");
        options.Scope.Add("email");
    });
builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});
builder.Services.AddScoped<IClaimsTransformation, RoleClaimsTransformer>();
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<Lab4User>>();

        // Создаем роли, если их нет
        string[] roleNames = { "Admin", "User" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Создаем администратора (опционально)
        var adminEmail = "admin@gmail.com";
        var adminUsername = "admin";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new Lab4User
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true // Подтверждаем email автоматически
            };

            // Создаем пользователя с паролем
            var createResult = await userManager.CreateAsync(adminUser, "Wasd_24032005");

            if (!createResult.Succeeded)
            {
                throw new Exception($"Ошибка создания администратора: {string.Join(", ", createResult.Errors)}");
            }

            // 3. Назначаем роль администратора
            var roleResult = await userManager.AddToRoleAsync(adminUser, "Admin");

            if (!roleResult.Succeeded)
            {
                throw new Exception($"Ошибка назначения роли: {string.Join(", ", roleResult.Errors)}");
            }

            Console.WriteLine("Администратор успешно создан!");
        }

    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ошибка при создании ролей");
    }
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();
