using Quartz;

namespace ToDoBackgoundService.Jobs
{
    public class DeleteToDoJob : IJob
    {
        private readonly ILogger<DeleteToDoJob> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public DeleteToDoJob(IHttpClientFactory httpClientFactory, ILogger<DeleteToDoJob> logger)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var jobKey = context.JobDetail.Key;
            var taskId = jobKey.Name.Split('-').LastOrDefault();

            if (int.TryParse(taskId, out var id))
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.DeleteAsync($"https://localhost:7258/api/ToDoList/DeleteToDoItem/{id}");

                _logger.LogInformation($"Job complite! -->");

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Successfully deleted ToDoItem with ID: {id}");
                }
                else
                {
                    _logger.LogInformation($"Failed to delete ToDoItem with ID: {id}");
                }
            }
        }
    }
}
