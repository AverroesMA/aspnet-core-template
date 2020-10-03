using Template.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Template.Infra
{
    public static class AppBuilderExtensions
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using var services = app.ApplicationServices.CreateScope();
            var dbContext = services.ServiceProvider.GetService<ApplicationDbContext>();
            dbContext.Database.Migrate();
        }
    }
}
