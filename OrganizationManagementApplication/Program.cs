using Microsoft.EntityFrameworkCore;
using OrganizationManagementApplication.Data;
using OrganizationManagementApplication.Models;
using OrganizationManagementApplication.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure Entity Framework Core to use SQL Server with the connection string from configuration
builder.Services.AddDbContext<OrganizationManagementContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ReflectionSerializationService>();
builder.Services.AddScoped<ReflectionHelperService>();
builder.Services.AddAutoMapper(typeof(EmployeeProfile).Assembly);


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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
