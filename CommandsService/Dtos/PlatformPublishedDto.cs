namespace CommandsService.Dtos
{
	// This will be sent by the PlatformsService and we will receive it from the Message Bus
	public class PlatformPublishedDto
	{
		public int Id { get; set; }

		public string? Name { get; set; }

		public string? Event { get; set; }
	}
}