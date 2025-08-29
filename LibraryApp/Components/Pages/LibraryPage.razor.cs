/*
using LibraryManagement.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Components.Pages
{
    public partial class DbContextPage : ComponentBase
    {
        [Inject] public IDbContextFactory<LibraryDbContext> DbFactory { get; set; }

        protected LibraryDbContext dbContext;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            if (dbContext == null)
            {
                dbContext = await DbFactory.CreateDbContextAsync();
                await dbContext.SeedAsync();
            }
        }
    }
}
*/
