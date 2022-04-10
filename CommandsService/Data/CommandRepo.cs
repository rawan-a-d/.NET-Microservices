using CommandsService.Models;

namespace CommandsService.Data
{
	public class CommandRepo : ICommandRepo
	{
		private readonly AppDbContext _context;

		public CommandRepo(AppDbContext context)
		{
			_context = context;
		}

		public void CreateCommand(int platformId, Command command)
		{
			// command not provided
			if (command == null)
			{
				throw new ArgumentNullException(nameof(command));
			}

			// add platformId (provided in the url) to the command object
			command.PlatformId = platformId;

			_context.Commands.Add(command);
		}

		public void CreatePlatform(Platform platform)
		{
			if (platform == null)
			{
				throw new ArgumentNullException(nameof(platform));
			}

			_context.Platforms.Add(platform);
		}

		public bool ExternalPlatformExists(int externalPlatformId)
		{
			return _context.Platforms.Any(p => p.ExternalId == externalPlatformId);
		}

		public IEnumerable<Platform> GetAllPlatforms()
		{
			return _context.Platforms.ToList();
		}

		public Command GetCommand(int platformId, int commandId)
		{
			return _context.Commands
				.Where(c => c.PlatformId == platformId && c.Id == commandId).FirstOrDefault();
		}

		public IEnumerable<Command> GetCommandsForPlatform(int platformId)
		{
			return _context.Commands
				.Where(c => c.PlatformId == platformId)
				.OrderBy(c => c.Platform.Name);
		}

		public bool PlatformExists(int platformId)
		{
			return _context.Platforms.Any(p => p.Id == platformId);
		}

		public bool SaveChanges()
		{
			return (_context.SaveChanges() >= 0);
		}
	}
}