namespace ToDoBackgoundService.Abstractions
{
    public interface IMessageBusListener
    {
        Task StartListeningAsync(CancellationToken cancellationToken);
    }
}
