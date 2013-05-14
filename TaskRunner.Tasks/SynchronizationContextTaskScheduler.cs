using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TaskRunner.Tasks
{
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
            _tasks.Enqueue(task);

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
