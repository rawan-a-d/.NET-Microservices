namespace PlatformService.Dtos
{
	// The platform that needs to be published to the Message Bus
	public class PlatformPublishedDto
	{
		public int Id { get; set; }

		public string? Name { get; set; }

		// type of event
		public string? Event { get; set; }
	}
}