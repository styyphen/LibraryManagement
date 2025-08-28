using Data;
using LibraryApp;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.EntityFrameworkCore;
using Radzen;
using Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add RootComponents - This is crucial for Blazor WebAssembly
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddRadzenComponents();

// For WebAssembly, we can't use SQLite directly in the browser
// You'll need to either:
// 1. Use an API to communicate with a server that handles database operations
// 2. Or use browser-based storage like IndexedDB

// Remove the SQLite configuration for WebAssembly
// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        corsPolicy =>
        {
            corsPolicy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

// Add DbContext
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Instead, add HttpClient for API calls
builder.Services.AddScoped(sp => new HttpClient { 
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) 
});
builder.Services.AddApiAuthorization();
builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<LenderService>();
builder.Services.AddScoped<LoanService>();

var app = builder.Build();

// Initialize database with seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<LibraryDbContext>();
        context.Database.EnsureCreated();
        Seeder.Seed(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

await app.RunAsync();