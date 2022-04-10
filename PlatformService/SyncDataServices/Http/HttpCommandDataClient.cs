using System.Text;
using System.Text.Json;
using PlatformService.Dtos;

namespace PlatformService.SyncDataServices.Http
{
	public class HttpCommandDataClient : ICommandDataClient
	{
		private readonly HttpClient _httpClient;
		private readonly IConfiguration _configuration;

		// configuration: gives access to config file
		public HttpCommandDataClient(HttpClient httpClient, IConfiguration configuration)
		{
			_httpClient = httpClient;
			_configuration = configuration;
		}


		public async Task SendPlatformToCommand(PlatformReadDto platform)
		{
			// create request payload
			var httpContent = new StringContent(
				JsonSerializer.Serialize(platform),
				Encoding.UTF8,
				"application/json"
			);

			// send post request
			//var response = await _httpClient.PostAsync("http://localhost:6000/api/c/platforms", httpContent);
			var response = await _httpClient.PostAsync(_configuration["CommandService"], httpContent);

			if (response.IsSuccessStatusCode)
			{
				Console.WriteLine("--> Sync POST to CommandService was OK!");
			}
			else
			{
				Console.WriteLine("--> Sync POST to CommandService was NOT OK!");
			}
		}
	}
}