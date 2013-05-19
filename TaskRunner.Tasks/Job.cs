using System;
using System.Diagnostics;
using System.Threading;

namespace TaskRunner.Tasks
{
    public interface Job
    {
        int Id { get; set; }
        int? DependencyId { get; set; }
        string Name { get; set; }
        int MaxDurationSeconds { get; set; }
        int MaxDurationMinutes { get; }

        JobHistory Execute();
    }

    public abstract class JobBase : Job
    {
        public JobBase(int id, string name, int maxDurationSeconds, int? dependencyId = null)
        {
            Id = id;
            DependencyId = dependencyId;
            Name = name;
            MaxDurationSeconds = maxDurationSeconds;
        }

        public int Id { get; set; }
        public int? DependencyId { get; set; }
        public string Name { get; set; }
        public int MaxDurationSeconds { get; set; }
        public int MaxDurationMinutes { get { return MaxDurationSeconds * 60; } }

        protected abstract void OnExecuteBegin();
        protected abstract void OnExecuteEnd();
        protected abstract void ExecuteJob();
        protected abstract JobHistory GetResult();

        public JobHistory Execute()
        {
            OnExecuteBegin();
            ExecuteJob();
            OnExecuteEnd();
            return GetResult();
        }
    }

    public class DefaultJob : JobBase
    {
        public DefaultJob(int id, string name, int maxDurationMinutes, int? dependencyId = null)
            : base(id, name, maxDurationMinutes, dependencyId)
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

        protected override JobHistory GetResult()
        {
            return new JobHistory
            {
                JobId = Id,
                ActivityTime = DateTime.Now,
                Successful = Timer.Elapsed.TotalSeconds < MaxDurationSeconds
            };
        }
    }

    public class JobHistory
    {
        public int JobId { get; set; }
        public DateTime ActivityTime { get; set; }
        public bool Successful { get; set; }
        public string Error { get; set; }

        public override string ToString()
        {
            return string.Format("JobId: [{0}], ActivityTime: [{1}], Successful: [{2}], Error: [{3}]",
                JobId, ActivityTime.ToString("MM/dd/yyyy hh:mm:ss"), Successful, Error);
        }
    }

    public static class JobHelpers
    {
        public static bool HasDependency(this Job job)
        {
            return job.DependencyId.HasValue;
        }
    }
}
