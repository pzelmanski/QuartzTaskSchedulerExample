using Quartz;
using Quartz.Impl;
using Quartz.Logging;
using Quartz.Spi;
using QuartzTaskScheduler.ScheduledTask;
using LogLevel = Quartz.Logging.LogLevel;

namespace QuartzTaskScheduler;

public static class MyTaskScheduler
{
    public static async Task StartBackgroundJobScheduler(WebApplication app)
    {
        LogProvider.SetCurrentLogProvider(new ConsoleLogProvider());

        var scheduler = await new StdSchedulerFactory().GetScheduler();
        await scheduler.Start();
        scheduler.JobFactory = new MyJobFactory(app.Services);

        // job class (ScheduledTaskService) has to implement IJob - having execute method, called on trigger
        var job = JobBuilder
            .Create<ScheduledTaskService>()
            .WithIdentity("job1", "b1")
            .Build();

        var trigger = TriggerBuilder.Create()
            .StartNow()
            .WithIdentity("trigger1", "b1")
            .WithSimpleSchedule(x => x
                .WithIntervalInSeconds(10)
                .RepeatForever())
            .Build();

        await scheduler.ScheduleJob(job, trigger);
    }

    // simple log provider to get something to the console
    private class ConsoleLogProvider : ILogProvider
    {
        public Logger GetLogger(String name)

        {
            return (level, func, exception, parameters) =>
            {
                if (level >= LogLevel.Info && func != null)
                {
                    Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] [" + level + "] " + func(),
                        parameters);
                }

                return true;
            };
        }

        public IDisposable OpenNestedContext(string message)
        {
            throw new NotImplementedException();
        }

        public IDisposable OpenMappedContext(string key, object value, bool destructure = false)
        {
            throw new NotImplementedException();
        }
    }

    private class MyJobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public MyJobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var jobDetail = bundle.JobDetail;
            var jobType = jobDetail.JobType;
            try
            {
                return (IJob)_serviceProvider.GetService(bundle.JobDetail.JobType)!;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Problem instantiating class {jobType.FullName}:\n{e}");
                throw;
            }
        }

        public void ReturnJob(IJob job)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (job is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}