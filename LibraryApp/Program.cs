using Data;
using LibraryApp;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Instead, add HttpClient for API calls
builder.Services.AddScoped(sp => new HttpClient { 
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) 
});

// Add RootComponents - This is crucial for Blazor WebAssembly
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Add DbContext
builder.Services.AddDbContextFactory<LibraryDbContext>();
builder.Services.AddRadzenComponents();

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