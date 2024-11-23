using ToDoBackgoundService.Abstractions;

namespace ToDoBackgoundService
{
    public class ToDoCleanerService : BackgroundService
    {
        private readonly IMessageBusListener _messageBusListener;

        public ToDoCleanerService(IMessageBusListener messageBusListener)
        {
            _messageBusListener = messageBusListener;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var listeningTask = _messageBusListener.StartListeningAsync(stoppingToken);

            await Task.Delay(Timeout.Infinite, stoppingToken);

            await listeningTask;
        }
    }
}
