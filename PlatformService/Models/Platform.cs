using System.ComponentModel.DataAnnotations;

// Internal representation of the data
namespace PlatformService.Models
{
	public class Platform
	{
		[Key] // database key
		[Required]
		public int Id { get; set; }

		[Required]
		public string? Name { get; set; }

		[Required]
		public string? Publisher { get; set; }

		[Required]
		public string? Cost { get; set; }
	}
}