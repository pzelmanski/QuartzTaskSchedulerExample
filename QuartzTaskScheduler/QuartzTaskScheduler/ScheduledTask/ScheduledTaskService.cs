using Quartz;

namespace QuartzTaskScheduler.ScheduledTask;

public class ScheduledTaskService : IJob
{
    private readonly ScheduledTaskRepository _repository;

    public ScheduledTaskService(ScheduledTaskRepository repository)
    {
        _repository = repository;
    }

    private Task DoWork()
    {
        Console.WriteLine($"{DateTime.UtcNow:hh:mm:ss} | Doing work at {nameof(ScheduledTaskService)}");
        return _repository.DoRepositoryWork();
    }

    public Task Execute(IJobExecutionContext context)
    {
        return DoWork();
    }
}