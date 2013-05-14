using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TaskRunner.Tasks
{
    public class SynchronizationContextTaskScheduler : TaskScheduler
    {
        private SynchronizationContext _context;

        public SynchronizationContextTaskScheduler()
            : this(SynchronizationContext.Current)
        {
            Tasks = new ConcurrentQueue<Task>();
        }

        public SynchronizationContextTaskScheduler(SynchronizationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            _context = context;

            Tasks = new ConcurrentQueue<Task>();
        }

        public override int MaximumConcurrencyLevel
        {
            get { return 1; }
        }

        protected ConcurrentQueue<Task> Tasks { get; set; }

        protected override void QueueTask(Task task)
        {
            Tasks.Enqueue(task);

            _context.Post(delegate
            {
                Task toExecute;
                if (Tasks.TryDequeue(out toExecute)) TryExecuteTask(toExecute);
            }, null);
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return SynchronizationContext.Current == _context && TryExecuteTask(task);
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return Tasks.ToArray();
        }
    }

    public class JobTaskScheduler : SynchronizationContextTaskScheduler
    {
        public JobTaskScheduler(ConcurrentQueue<Task> jobQueue)
            : base(SynchronizationContext.Current)
        {
            Tasks = jobQueue;
        }
    }
}
