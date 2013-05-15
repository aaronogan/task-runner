using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskRunner.Tasks
{
    public interface Job
    {
        int Id { get; set; }
        string Name { get; set; }
        int MaxDurationMinutes { get; set; }

        JobResult Execute();
    }

    public abstract class JobBase : Job
    {
        public JobBase(int id, string name, int maxDurationMinutes)
        {
            Id = id;
            Name = name;
            MaxDurationMinutes = maxDurationMinutes;
        }

        public int Id { get; set; }
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
        public DefaultJobImpl(int id, string name, int maxDurationMinutes)
            : base(id, name, maxDurationMinutes)
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

    public class DependencyJobImpl : DefaultJobImpl
    {
        public DependencyJobImpl(int id, string name, int maxDurationMinutes, int? dependencyId = null)
            : base(id, name, maxDurationMinutes)
        {
            DependencyId = dependencyId;
        }

        public int? DependencyId { get; protected set; }
    }

    public class JobResult
    {
        public bool Successful { get; set; }
    }
}
