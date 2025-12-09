using Application.Interfaces;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Auth;
using Infrastructure.Excel;
using Infrastructure.Persistence;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Web.Admin.Seed;
using Web.Admin.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<TalentoPlusDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<TalentoPlusDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IProgramRepository, ProgramRepository>();
builder.Services.AddScoped<IExcelEmployeeReader, ExcelEmployeeReader>();
builder.Services.AddHttpClient<ApiClient>(c =>
{
    c.BaseAddress = new Uri("http://localhost:5164");
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
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
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    await AdminSeeder.SeedAsync(scope.ServiceProvider);
}

app.Run();