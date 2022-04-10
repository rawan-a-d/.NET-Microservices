using System.Text;
using System.Text.Json;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices
{
	public class MessageBusClient : IMessageBusClient
	{
		private readonly IConfiguration _configuration;
		private readonly IConnection _connection;
		private readonly IModel _channel;

		public MessageBusClient(IConfiguration configuration)
		{
			_configuration = configuration;
			var factory = new ConnectionFactory()
			{
				HostName = _configuration["RabbitMQHost"],
				Port = int.Parse(_configuration["RabbitMQPort"])
			};

			try
			{
				// connect to message bus
				_connection = factory.CreateConnection();
				// create channel
				_channel = _connection.CreateModel();
				// declare exchange in our channel
				_channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

				// subscribe to shut down event (OPTIONAL)
				_connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

				Console.WriteLine("--> Connected to MessageBus");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"--> Could not connect to the Message Bus: {ex.Message}");
			}
		}

		public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
		{
			// create a message object (a serialized version of the platformPublishedDto)
			var message = JsonSerializer.Serialize(platformPublishedDto);

			// check if connection is open
			if (_connection.IsOpen)
			{
				Console.WriteLine("--> RabbitMQ Connection Open, sending message...");
				// send the message
				SendMessage(message);
			}
			else
			{
				Console.WriteLine("--> RabbitMQ connection closed, not sending");
			}
		}

		// Send message can be used to send any type of message
		private void SendMessage(string message)
		{
			// create body of message (byte array)
			var body = Encoding.UTF8.GetBytes(message);

			// publish message (can put exchange name into config file)
			_channel.BasicPublish(
				exchange: "trigger",
				routingKey: "",
				basicProperties: null,
				body: body
			);

			Console.WriteLine($"--> We have sent {message}");
		}

		// Clean up when our class dies
		public void Dispose()
		{
			Console.WriteLine("MessageBus Disposed");
			// close the channel if it is open
			if (_channel.IsOpen)
			{
				_channel.Close();
				_connection.Close();
			}
		}

		private void RabbitMQ_ConnectionShutdown(Object sender, ShutdownEventArgs e)
		{
			Console.WriteLine("--> RabbitMQ Connection Shutdown");
		}
	}
}