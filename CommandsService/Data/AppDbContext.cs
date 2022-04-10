using CommandsService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandsService.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
		{

		}

		public DbSet<Platform> Platforms { get; set; }
		public DbSet<Command> Commands { get; set; }

		// Explicilty specify relationships to avoid any unwanted behavior
		// ASP.net can do that automatically
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Platform
			modelBuilder
				.Entity<Platform>()
				.HasMany(p => p.Commands)
				.WithOne(p => p.Platform!)
				.HasForeignKey(p => p.PlatformId);

			// Command
			modelBuilder
				.Entity<Command>()
				// platform object
				.HasOne(c => c.Platform)
				// list of commands 
				.WithMany(c => c.Commands)
				// forein key
				.HasForeignKey(c => c.PlatformId);
		}
	}
}