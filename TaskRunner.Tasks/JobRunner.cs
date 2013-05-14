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
            OnExecuteBegin();

            var results = new List<JobResult>();
            var jobTasks = new List<Task<JobResult>>();
            foreach (var job in jobs)
            {
                jobTasks.Add(
                        Factory.StartNew(() => {
                            var result = job.Execute();
                            results.Add(result);
                            return result;
                        })
                    );
            }

            Task.WaitAll(jobTasks.ToArray<Task<JobResult>>());

            if (jobTasks.Count > 0)
            {
                var completedTasks = Factory.ContinueWhenAll(jobTasks.ToArray<Task>(), _ =>
                    {
                        OnExecuteEnd();
                    });
            }
            else
            {
                OnExecuteEnd();
            }

            return results;
        }

        protected virtual void OnExecuteBegin()
        {
        }

        protected virtual void OnExecuteEnd()
        {
        }
    }
}
