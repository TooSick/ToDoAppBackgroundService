using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using ToDoBackgoundService.Abstractions;
using ToDoBackgoundService.Models;

namespace ToDoBackgoundService.AsincDataListeners
{
    internal class RabbitMqListener : IMessageBusListener
    {
        private readonly ILogger<RabbitMqListener> _logger;
        private readonly IConnectionFactory _connectionFactory;
        private readonly ISchedulerService<ToDoItem> _schedulerService;

        public RabbitMqListener(ISchedulerService<ToDoItem> schedulerService, ILogger<RabbitMqListener> logger)
        {
            _schedulerService = schedulerService;
            _connectionFactory = new ConnectionFactory()
            {
                HostName = "localhost",
            };
            _logger = logger;
        }

        public async Task StartListeningAsync(CancellationToken cancellationToken)
        {
            var channel = await GetChannelAsync();
            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                var toDoItem = JsonSerializer.Deserialize<ToDoItem>(message);

                _logger.LogInformation($"[RabbitMqListener] Received message: {message}");
                _logger.LogInformation($"Json deserialize result: Id - {toDoItem.Id}; DueDate - {toDoItem.DueDate}");

                if (toDoItem != null)
                    await _schedulerService.ScheduleTaskAsync(toDoItem);
            };

            await channel.BasicConsumeAsync("ToDoItems", autoAck: true, consumer: consumer);

            cancellationToken.Register(() =>
            {
                channel.Dispose();
            });
        }

        private async Task<IChannel> GetChannelAsync()
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: "ToDoItems", durable: false, exclusive: false, autoDelete: false,
                arguments: null);

            return channel;
        }
    }
}
