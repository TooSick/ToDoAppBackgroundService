using Quartz;
using ToDoBackgoundService.Abstractions;
using ToDoBackgoundService.Jobs;
using ToDoBackgoundService.Models;

namespace ToDoBackgoundService.Services
{
    public class QuartzSchedulerService : ISchedulerService<ToDoItem>
    {
        private readonly ILogger<QuartzSchedulerService> _logger;
        private readonly ISchedulerFactory _schedulerFactory;

        public QuartzSchedulerService(ISchedulerFactory schedulerFactory, ILogger<QuartzSchedulerService> logger)
        {
            _logger = logger;
            _schedulerFactory = schedulerFactory;
        }

        public async Task ScheduleTaskAsync(ToDoItem item)
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            var deleteTime = new DateTimeOffset(item.DueDate.AddHours(8), TimeSpan.Zero);

            var existingJob = await scheduler.GetJobDetail(new JobKey($"DeleteToDo-{item.Id}"));

            if (existingJob != null)
                await scheduler.DeleteJob(new JobKey($"DeleteToDo-{item.Id}"));

            var job = JobBuilder.Create<DeleteToDoJob>()
                .WithIdentity($"DeleteToDo-{item.Id}")
                .Build();

            var trigger = TriggerBuilder.Create()
                .StartAt(deleteTime)
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }
    }
}
