using System.Text.Json;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService.EventProcessing
{
	// Takes a message, determines what it is and processes it
	public class EventProcessor : IEventProcessor
	{
		private readonly IServiceScopeFactory _scopeFactory;
		private readonly IMapper _mapper;

		public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
		{
			_scopeFactory = scopeFactory;
			_mapper = mapper;
		}

		// Process the event
		public void ProcessEvent(string message)
		{
			var eventType = DetermineEvent(message);

			switch (eventType)
			{
				case EventType.PlatformPublished:
					// add platform
					AddPlatform(message);
					break;
				default:
					break;
			}
		}

		// Ensures that we understand the event we got
		private EventType DetermineEvent(string notificationMessage)
		{
			Console.WriteLine("--> Determining Event");

			// deserialize string to object (GenericEventDto)
			var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

			switch (eventType?.Event)
			{
				case "Platform_Published":
					Console.WriteLine("--> Platform Published Event Detected");
					return EventType.PlatformPublished;
				default:
					Console.WriteLine("--> Could not determine the event type");
					return EventType.Undetermined;
			}
		}

		// Add platform to DB if it doesn't already exist
		private void AddPlatform(string platformPublishedMessage)
		{
			using (var scope = _scopeFactory.CreateScope())
			{
				// get a reference to ICommandRepo
				var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();

				// deserialize string to object (PlatformPublishDto)
				var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

				try
				{
					var platformModel = _mapper.Map<Platform>(platformPublishedDto);

					// if platform doesn't exist
					if (!repo.ExternalPlatformExists(platformModel.ExternalId))
					{
						repo.CreatePlatform(platformModel);
						repo.SaveChanges();
						Console.WriteLine("--> Platform added!");
					}
					else
					{
						Console.WriteLine("--> Platform already exists");
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine($"--> Could not add Platform to DB {ex.Message}");
				}
			}
		}
	}

	enum EventType
	{
		PlatformPublished,
		Undetermined
	}
}