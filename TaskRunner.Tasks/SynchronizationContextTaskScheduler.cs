using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskRunner.Tasks
{
    public class SynchronizationContextTaskScheduler : TaskScheduler
    {
        private ConcurrentDictionary<int, Task> _taskDictionary = new ConcurrentDictionary<int, Task>();
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
            return SynchronizationContext.Current == _context &&
                      TryExecuteTask(task);
        }

        public override int MaximumConcurrencyLevel
        {
            get { return 1; }
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return _tasks.ToArray();
        }
    }
}
