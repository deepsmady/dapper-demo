using DapperDemo.Data;
using DapperDemo.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
#pragma warning disable CS8604 // Possible null reference argument.
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
#pragma warning restore CS8604 // Possible null reference argument.
//builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
//builder.Services.AddScoped<ICompanyRepository, CompanyRepositorySP>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepositoryContib>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IBonusRepository, BonusRepository>();
builder.Services.AddScoped<IDapperSprocRepo, DapperSprocRepo>();
builder.Services.AddControllersWithViews();

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
