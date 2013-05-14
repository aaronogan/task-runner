using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskRunner.Tasks
{
    public interface JobRunner
    {
        IEnumerable<JobResult> Execute(IEnumerable<Job> jobs);
    }

    public class DefaultJobRunnerImpl : JobRunner
    {
        protected virtual TaskScheduler Scheduler
        {
            get { return TaskScheduler.FromCurrentSynchronizationContext(); }
        }

        protected virtual TaskFactory Factory
        {
            get { return Task.Factory; }
        }

        public IEnumerable<JobResult> Execute(IEnumerable<Job> jobs)
        {
            var jobTasks = new List<Task<JobResult>>();
            
            foreach (var job in jobs)
            {
                jobTasks.Add(Factory.StartNew(() => {
                    return job.Execute();
                }));
            }

            Task.WaitAll(jobTasks.ToArray<Task<JobResult>>());

            return jobTasks.Select(x => x.Result);
        }
    }
}
