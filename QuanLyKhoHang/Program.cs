using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoHang.Models;
using QuanLyKhoHang.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<QuanLyKhoHang.Models.ApplicationDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
        .AddDefaultTokenProviders()
        .AddDefaultUI()
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDBContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

builder.Services.AddScoped<IProductRepository, EFProductRepository>();
builder.Services.AddScoped<ICategoryRepository, EFCategoryRepository>();
builder.Services.AddScoped<ISupplierRepository, EFSupplierRepository>();
builder.Services.AddScoped<ICustomerRepository, EFCustomerRepository>();
builder.Services.AddScoped<IStockInRepository, EFStockInRepository>();
builder.Services.AddScoped<IStockInDetailRepository, EFStockInDetailRepository>();
builder.Services.AddScoped<IStockOutRepository, EFStockOutRepository>();
builder.Services.AddScoped<IStockOutDetailRepository, EFStockOutDetailRepository>();
builder.Services.AddHostedService<RoleInitializerHostedService>();

builder.Services.AddRazorPages();

var app = builder.Build();

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

app.MapRazorPages();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "Admin",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();
