using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskRunner.Tasks
{
    public interface JobScheduler
    {
        ConcurrentQueue<Job> GetScheduledJobs();
    }

    public abstract class JobSchedulerBase : JobScheduler
    {
        public JobSchedulerBase(IEnumerable<Job> toSchedule)
        {
            ToSchedule = new ConcurrentQueue<Job>(toSchedule);
        }

        protected ConcurrentQueue<Job> ToSchedule { get; set; }

        public abstract ConcurrentQueue<Job> GetScheduledJobs();
    }

    public class DependencyJobScheduler : JobSchedulerBase, JobScheduler
    {
        public DependencyJobScheduler(IEnumerable<Job> toSchedule)
            : base(toSchedule)
        {
        }

        public override ConcurrentQueue<Job> GetScheduledJobs()
        {
            return ToSchedule;
        }
    }

    public class SynchronizationContextTaskScheduler : TaskScheduler
    {
        private ConcurrentQueue<Task> _tasks = new ConcurrentQueue<Task>();
        private SynchronizationContext _context;

        public SynchronizationContextTaskScheduler() :
            this(SynchronizationContext.Current)
        {
        }

        public SynchronizationContextTaskScheduler(SynchronizationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            _context = context;
        }

        public override int MaximumConcurrencyLevel
        {
            get { return 1; }
        }

        protected override void QueueTask(Task task)
        {
            // Add the task to the collection 
            _tasks.Enqueue(task);

            // Queue up a delegate that will dequeue and execute a task 
            _context.Post(delegate
            {
                Task toExecute;
                if (_tasks.TryDequeue(out toExecute)) TryExecuteTask(toExecute);
            }, null);
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return SynchronizationContext.Current == _context && TryExecuteTask(task);
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return _tasks.ToArray();
        }
    }
}
