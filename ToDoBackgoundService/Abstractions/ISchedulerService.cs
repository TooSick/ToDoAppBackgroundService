namespace ToDoBackgoundService.Abstractions
{
    internal interface ISchedulerService<T>
    {
        Task ScheduleTaskAsync(T item);
    }
}
