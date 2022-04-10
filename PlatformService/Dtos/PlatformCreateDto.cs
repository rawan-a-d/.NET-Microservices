// External representation of the data
// beneficial because:
// 1. some data shouldn't be exposed
// 2. create an abstraction layer, so if the actual models changes the Dto is not affected
using System.ComponentModel.DataAnnotations;

namespace PlatformService.Dtos {
	public class PlatformCreateDto {
		[Required]
		public string? Name { get; set; }
		
		[Required]
		public string? Publisher { get; set; }
		
		[Required]
		public string? Cost { get; set; }
	}
}
