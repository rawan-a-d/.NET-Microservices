// External representation of the data
// beneficial because:
// 1. some data shouldn't be exposed
// 2. create an abstraction layer, so if the actual models changes the Dto is not affected
namespace PlatformService.Dtos {
	public class PlatformReadDto {
		public int Id { get; set; }

		public string? Name { get; set; }

		public string? Publisher { get; set; }

		public string? Cost { get; set; }
	}
}
