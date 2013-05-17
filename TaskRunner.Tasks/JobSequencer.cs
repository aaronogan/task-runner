using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskRunner.Tasks
{
    [Obsolete("", true)]
    public interface JobSequencer<T> where T : Job
    {
        Queue<T> GetSequencedJobs(IEnumerable<T> jobs);
        Job GetNextJob();
    }

    [Obsolete("", true)]
    public class DefaultJobSequencer<T> : JobSequencer<T> where T : Job
    {
        public DefaultJobSequencer(JobRepository repository)
        {
            Repository = repository;
        }

        protected JobRepository Repository { get; set; }

        public Queue<T> GetSequencedJobs(IEnumerable<T> jobs)
        {
            var sorted = new List<T>(jobs).OrderBy(x => x.DependencyId);
            return new Queue<T>(sorted);
        }

        public virtual Job GetNextJob()
        {
            throw new NotImplementedException();
        }
    }
}
