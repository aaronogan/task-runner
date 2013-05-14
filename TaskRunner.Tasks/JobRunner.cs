using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TaskRunner.Tasks
{
    public interface JobRunner<T> where T : Job
    {
        IEnumerable<JobResult> Execute(IEnumerable<T> jobs);
    }

    public class DefaultJobRunnerImpl : JobRunner<Job>
    {
        public DefaultJobRunnerImpl()
            : this(SynchronizationContext.Current)
        {
        }

        public DefaultJobRunnerImpl(SynchronizationContext context)
        {
            Context = context;
        }

        protected SynchronizationContext Context { get; set; }

        private TaskScheduler _taskScheduler = null;
        protected virtual TaskScheduler Scheduler
        {
            get
            {
                if (_taskScheduler == null)
                {
                    _taskScheduler = new SynchronizationContextTaskScheduler(Context);
                }

                return _taskScheduler;
            }
        }

        private TaskFactory _factory = null;
        protected virtual TaskFactory Factory
        {
            get
            {
                if (_factory == null)
                {
                    _factory = new TaskFactory(Scheduler);
                }
                return _factory;
            }
        }

        public virtual IEnumerable<JobResult> Execute(IEnumerable<Job> jobs)
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

    public class DependencyJobRunnerImpl<T> : DefaultJobRunnerImpl where T : DependencyJobImpl
    {
        private JobSequencer<T> _jobSequencer;
        private ConcurrentQueue<Task> _taskQueue;

        public DependencyJobRunnerImpl(JobSequencer<T> sequencer)
        {
            _jobSequencer = sequencer;
        }

        public IEnumerable<JobResult> Execute(IEnumerable<T> jobs)
        {
            var queue = _jobSequencer.GetSequencedJobs(jobs);
            var jobTasks = new List<Task<JobResult>>();

            while (queue.Any())
            {
                jobTasks.Add(Factory.StartNew(() => {
                    T next = null;
                    if (queue.TryDequeue(out next))
                    {
                        return next.Execute();
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }));
            }

            Task.WaitAll(jobTasks.ToArray<Task<JobResult>>());
            
            return jobTasks.Select(x => x.Result);
        }

        public DependencyJobRunnerImpl(ConcurrentQueue<Task> taskQueue)
        {
            _taskQueue = taskQueue;
        }

        private TaskScheduler _taskScheduler = null;
        protected override TaskScheduler Scheduler
        {
            get
            {
                if (_taskScheduler == null)
                {
                    _taskScheduler = new JobTaskScheduler(_taskQueue);
                }

                return _taskScheduler;
            }
        }
    }
}
