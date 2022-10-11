namespace QuartzTaskScheduler.ScheduledTask;

public class ScheduledTaskRepository
{
    public Task DoRepositoryWork()
    {
        Task.Delay(1500);
        Console.WriteLine($"{DateTime.UtcNow:hh:mm:ss} | Doing work at {nameof(ScheduledTaskRepository)}");
        return Task.CompletedTask;
    }
}