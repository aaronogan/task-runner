using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskRunner.Tasks
{
    public class SequentialJobRunner : JobRunner
    {
        protected List<Job> Jobs { get; set; }

        public virtual void Execute()
        {
        
        }
    }

    public class JobBase : Job
    {
        public JobBase(string name, int maxDurationMinutes)
        {
            Name = name;
            MaxDurationMinutes = maxDurationMinutes;
        }

        public string Name { get; set; }
        public int MaxDurationMinutes { get; set; }

        public virtual JobResult Execute()
        {
            throw new NotImplementedException();
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

    public interface JobResult
    {
        bool Successful { get; }
    }
}
