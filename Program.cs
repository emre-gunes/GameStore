using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GameStore.Data;
using GameStore.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();

// Seed Default Data
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    string[] roleNames = { "Admin", "User" };
    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    var poweruser = new AppUser
    {
        UserName = "admin@gamestore.com",
        Email = "admin@gamestore.com",
        Name = "Admin",
        Surname = "User",
        EmailConfirmed = true
    };

    string userPWD = "Password123!";
    var _user = await userManager.FindByEmailAsync("admin@gamestore.com");
    if (_user == null)
    {
        var createPowerUser = await userManager.CreateAsync(poweruser, userPWD);
        if (createPowerUser.Succeeded)
        {
            await userManager.AddToRoleAsync(poweruser, "Admin");
        }
    }
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    // Update existing English records to Turkish
    var cats = dbContext.Categories.ToList();
    foreach(var c in cats) {
        if(c.Name == "Action") c.Name = "Aksiyon";
        if(c.Name == "Adventure") c.Name = "Macera";
        if(c.Name == "Strategy") c.Name = "Strateji";
    }
    await dbContext.SaveChangesAsync();

    var games = dbContext.Games.ToList();
    foreach(var g in games) {
        if(g.Price < 100) g.Price = g.Price * 35; // Convert small $ prices to TL
    }
    await dbContext.SaveChangesAsync();

    if (!dbContext.Categories.Any())
    {
        var actionCat = new Category { Name = "Aksiyon", Description = "Aksiyon oyunları" };
        var adventureCat = new Category { Name = "Macera", Description = "Macera oyunları" };
        var strategyCat = new Category { Name = "Strateji", Description = "Strateji oyunları" };

        dbContext.Categories.AddRange(actionCat, adventureCat, strategyCat);
        await dbContext.SaveChangesAsync();

        dbContext.Games.AddRange(
            new Game { Title = "Assasin Creed", Description = "Harika bir aksiyon oyunu.", Price = 840.00m, StockQuantity = 100, CategoryId = actionCat.Id, ImageUrl = "/assets/images/trending-01.jpg" },
            new Game { Title = "Cyberpunk 2077", Description = "Fütüristik açık dünya macerası.", Price = 1575.00m, StockQuantity = 50, CategoryId = adventureCat.Id, ImageUrl = "/assets/images/trending-02.jpg" },
            new Game { Title = "Age of Empires IV", Description = "Klasik strateji oyunu.", Price = 1050.00m, StockQuantity = 75, CategoryId = strategyCat.Id, ImageUrl = "/assets/images/trending-03.jpg" },
            new Game { Title = "Witcher 3", Description = "Epik fantastik RYO.", Price = 630.00m, StockQuantity = 200, CategoryId = actionCat.Id, ImageUrl = "/assets/images/trending-04.jpg" }
        );
        await dbContext.SaveChangesAsync();
    }
}

app.Run();
