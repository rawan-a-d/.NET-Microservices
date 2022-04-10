using System.ComponentModel.DataAnnotations;

namespace CommandsService.Models
{
	public class Command
	{
		[Key]
		[Required]
		public int Id { get; set; }

		[Required]
		public string? HowTo { get; set; }

		[Required]
		public string? CommandLine { get; set; }

		[Required]
		// foreign key reference to Platform
		public int PlatformId { get; set; }

		// navigation property (allows us to navigate between commands and platforms)
		public Platform? Platform { get; set; }
	}
}