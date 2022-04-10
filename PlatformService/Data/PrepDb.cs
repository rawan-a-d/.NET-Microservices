using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
	// For testing purposes (DONT PUSH TO PRODUCTION)
	// will be called in Program.cs
	// Generate migrations in SQL database
	public static class PrepDb
	{
		public static void PrepPopulation(IApplicationBuilder app, bool isProd)
		{
			using (var serviceScope = app.ApplicationServices.CreateScope())
			{
				SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProd);
			}
		}

		private static void SeedData(AppDbContext context, bool isProd)
		{
			// run migration if we are in production
			if (isProd)
			{
				Console.WriteLine("--> Attempting to apply migrations...");

				try
				{
					context.Database.Migrate();
				}
				catch (Exception ex)
				{
					Console.WriteLine($"--> Could not run migrations: {ex.Message}");
				}
			}

			// if no data
			if (!context.Platforms.Any())
			{
				Console.WriteLine("--> Seeding data...");

				context.Platforms.AddRange(
					new Platform() { Name = "Dot Net", Publisher = "Microsoft", Cost = "Free" },
					new Platform() { Name = "SQL Server Express", Publisher = "Microsoft", Cost = "Free" },
					new Platform() { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free" }
				);

				context.SaveChanges();
			}
			else
			{
				Console.WriteLine("--> We already have data");
			}
		}
	}
}