using Microsoft.EntityFrameworkCore;
using ProductCatalog.DB;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Register the DbContext with dependency injection, configuring it to use SQL Server
builder.Services.AddDbContext<ProductCatalogContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ProductCatalogDB")));

// Add services for controllers (Web API)
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts(); // Enable HSTS in non-development environments for security
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Map the Razor Pages routes
app.MapRazorPages();

// Map the Web API controller routes
app.MapControllers();

app.Run();
