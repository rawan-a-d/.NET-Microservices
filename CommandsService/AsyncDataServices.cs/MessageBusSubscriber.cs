using System.Text;
using CommandsService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandsService.AsyncDataServices
{
	// Event Listener
	// Long running task, will work in the backgroung and listen for any events
	public class MessageBusSubscriber : BackgroundService
	{
		private readonly IConfiguration _configuration;
		private readonly IEventProcessor _eventProcessor;
		private IConnection _connection;
		private IModel _channel;
		private string _queueName;

		public MessageBusSubscriber(
			IConfiguration configuration,
			IEventProcessor eventProcessor)
		{
			_configuration = configuration;
			_eventProcessor = eventProcessor;

			InitializeRabbitMQ();
		}

		// Connect and listen to RabbitMQ message bus
		private void InitializeRabbitMQ()
		{
			// connect to RabbitMQ
			var factory = new ConnectionFactory()
			{
				HostName = _configuration["RabbitMQHost"],
				Port = int.Parse(_configuration["RabbitMQPort"])
			};

			_connection = factory.CreateConnection();
			_channel = _connection.CreateModel();
			_channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
			_queueName = _channel.QueueDeclare().QueueName;
			_channel.QueueBind(
				queue: _queueName,
				exchange: "trigger",
				routingKey: "");

			Console.WriteLine("--> Listening on the Mesage Bus...");

			// subscribe to connection shutdown
			_connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
		}

		// all runnig task, waiting and listening to events
		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			// declare a stopping token
			stoppingToken.ThrowIfCancellationRequested();

			// consumer
			var consumer = new EventingBasicConsumer(_channel);

			// this triggers when there is an event on the message bus
			consumer.Received += (ModuleHandle, ea) =>
			{
				Console.WriteLine("--> Event Received");

				// body of message
				var body = ea.Body;
				// convert from byte array to string
				var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

				// process event
				_eventProcessor.ProcessEvent(notificationMessage);
			};

			// keep consuming
			_channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

			return Task.CompletedTask;
		}

		private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
		{
			Console.WriteLine("--> Connection Shutdown");
		}

		public override void Dispose()
		{
			if (_channel.IsOpen)
			{
				_channel.Close();
				_connection.Close();
			}

			base.Dispose();
		}
	}
}