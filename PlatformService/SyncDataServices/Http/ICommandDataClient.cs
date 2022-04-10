using PlatformService.Dtos;

namespace PlatformService.SyncDataServices.Http
{
	// Send request Sync (request/response cycle) to Commands service
	public interface ICommandDataClient
	{
		Task SendPlatformToCommand(PlatformReadDto platform);
	}
}