
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplicationList.ApplicationDataBase;
using WebApplicationList.Models.Enitity;
using WebApplicationList.Services;
using WebApplicationList.Services.Models;

var builder = WebApplication.CreateBuilder(args);

string? connection = builder.Configuration.GetConnectionString("LocalConnection");

builder.Services.AddDbContext<ApplicationDb>(options => options.UseSqlServer(connection!, sqlServerOptionsAction: sqlServerOptions =>
{
    sqlServerOptions.EnableRetryOnFailure(
    maxRetryCount: 2,
    maxRetryDelay: TimeSpan.FromSeconds(10),
    errorNumbersToAdd: null);
}));

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequiredLength = 5;
    options.User.RequireUniqueEmail = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
}).AddEntityFrameworkStores<ApplicationDb>();

//builder.Services.ConfigureApplicationCookie(options =>
//{
//    options.ExpireTimeSpan = TimeSpan.FromHours(1);
//});

builder.Services.AddScoped<IAuthorization, Authorization>()
    .AddScoped<IProfileUser, ProfileUser>()
    .AddScoped<IProjectSetting, ProjectSetting>()
    .AddTransient<ISearch, SearchRepository>();
    
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.AllowAnyOrigin();
    });
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Main}/{action=Index}/{id?}");

app.Run();
