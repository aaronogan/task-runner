using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskRunner.Tasks
{
    public class SequentialJobRunner : JobRunner
    {
        protected List<Job> Jobs { get; set; }

        public virtual void Execute()
        {
            foreach (var job in Jobs)
            {
                job.Execute();
            }
        }
    }

    public abstract class JobBase : Job
    {
        public JobBase(string name, int maxDurationMinutes)
        {
            Name = name;
            MaxDurationMinutes = maxDurationMinutes;
        }

        public string Name { get; set; }
        public int MaxDurationMinutes { get; set; }

        protected abstract void OnExecuteBegin();
        protected abstract void OnExecuteEnd();
        protected abstract void ExecuteJob();
        protected abstract JobResult GetResult();

        public JobResult Execute()
        {
            OnExecuteBegin();
            ExecuteJob();
            OnExecuteEnd();
            return GetResult();
        }
    }

    public class DefaultJobImpl : JobBase
    {
        public DefaultJobImpl(string name, int maxDurationMinutes)
            : base(name, maxDurationMinutes)
        {
            Timer = new Stopwatch();
        }

        protected Stopwatch Timer { get; private set; }

        protected override void OnExecuteBegin()
        {
            Timer.Start();
        }

        protected override void OnExecuteEnd()
        {
            Timer.Stop();
        }

        protected override void ExecuteJob()
        {
        }

        protected override JobResult GetResult()
        {
            return new JobResult
            {
                Successful = Timer.Elapsed.TotalMinutes < MaxDurationMinutes
            };
        }
    }

    public interface JobRunner
    {
        void Execute();
    }

    public interface Job
    {
        string Name { get; set; }
        int MaxDurationMinutes { get; set; }

        JobResult Execute();
    }

    public class JobResult
    {
        public bool Successful { get; set; }
    }
}
