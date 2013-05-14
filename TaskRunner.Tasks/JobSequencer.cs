using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaskRunner.Tasks
{
    public interface JobSequencer<T> where T : Job
    {
        ConcurrentQueue<T> ToSchedule { get; }
        ConcurrentQueue<T> GetSequencedJobs();
    }

    public class DependencyJobSequencer : JobSequencer<DependencyJobImpl>
    {
        public DependencyJobSequencer(IEnumerable<DependencyJobImpl> toSchedule)
        {
            ToSchedule = new ConcurrentQueue<DependencyJobImpl>(toSchedule);
        }

        public ConcurrentQueue<DependencyJobImpl> ToSchedule { get; protected set; }

        public ConcurrentQueue<DependencyJobImpl> GetSequencedJobs()
        {
            var sorted = new List<DependencyJobImpl>(ToSchedule).OrderBy(x => x.DependencyId);
            return new ConcurrentQueue<DependencyJobImpl>(sorted);
        }
    }
}
