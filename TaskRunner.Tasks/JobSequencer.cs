using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaskRunner.Tasks
{
    public interface JobSequencer<T> where T : Job
    {
        ConcurrentQueue<T> GetSequencedJobs(IEnumerable<T> jobs);
    }

    public class DependencyJobSequencer : JobSequencer<DependencyJobImpl>
    {
        public ConcurrentQueue<DependencyJobImpl> GetSequencedJobs(IEnumerable<DependencyJobImpl> jobs)
        {
            var sorted = new List<DependencyJobImpl>(jobs).OrderBy(x => x.DependencyId);
            return new ConcurrentQueue<DependencyJobImpl>(sorted);
        }
    }
}
