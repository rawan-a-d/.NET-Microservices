using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
	[Route("api/c/platforms/{platformId}/[controller]")]
	[ApiController]
	public class CommandsController : ControllerBase
	{
		private readonly ICommandRepo _repository;
		private readonly IMapper _mapper;

		public CommandsController(ICommandRepo repository, IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}

		[HttpGet]
		public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
		{
			Console.WriteLine($"--> Hit GetCommandsForPlatform: {platformId}");

			// check if platform exists
			var platformExists = _repository.PlatformExists(platformId);

			if (!platformExists)
			{
				return NotFound();
			}

			// get commands
			var commands = _repository.GetCommandsForPlatform(platformId);

			return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
		}

		[HttpGet("{commandId}", Name = "GetCommandForPlatform")]
		public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
		{
			Console.WriteLine($"--> Hit GetCommandForPlatform: {platformId} / {commandId}");

			// check if platform exists
			var platformExists = _repository.PlatformExists(platformId);

			if (!platformExists)
			{
				return NotFound();
			}

			// get command
			var command = _repository.GetCommand(platformId, commandId);

			if (command == null)
			{
				return NotFound();
			}

			return Ok(_mapper.Map<CommandReadDto>(command));
		}

		[HttpPost]
		public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandCreateDto)
		{
			Console.WriteLine($"--> Hit CreateCommandForPlatform: {platformId}");

			// check if platform exists
			var platformExists = _repository.PlatformExists(platformId);

			if (!platformExists)
			{
				return NotFound();
			}

			// create command
			var commandModel = _mapper.Map<Command>(commandCreateDto);

			_repository.CreateCommand(platformId, commandModel);
			_repository.SaveChanges();

			var commandReadDto = _mapper.Map<CommandReadDto>(commandModel);

			return CreatedAtRoute(
				nameof(GetCommandForPlatform),
				new { platformId = platformId, commandId = commandReadDto.Id },
				commandReadDto
			);
		}
	}
}