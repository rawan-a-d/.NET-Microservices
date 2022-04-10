using System.ComponentModel.DataAnnotations;

namespace CommandsService.Models
{
	public class Platform
	{
		[Key]
		[Required]
		public int Id { get; set; }

		// primary key from platforms service
		[Required]
		public int ExternalId { get; set; }

		[Required]
		public string? Name { get; set; }

		// navigation property (allows us to navigate between commands and platforms)
		public ICollection<Command>? Commands { get; set; } = new List<Command>();
	}
}