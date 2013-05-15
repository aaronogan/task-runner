using System.Collections.Generic;
using System.Linq;

namespace TaskRunner.Tasks
{
    public interface JobSequencer<T> where T : Job
    {
        Queue<T> GetSequencedJobs(IEnumerable<T> jobs);
    }

    public class DependencyJobSequencer : JobSequencer<DependencyJobImpl>
    {
        public Queue<DependencyJobImpl> GetSequencedJobs(IEnumerable<DependencyJobImpl> jobs)
        {
            var sorted = new List<DependencyJobImpl>(jobs).OrderBy(x => x.DependencyId);
            return new Queue<DependencyJobImpl>(sorted);
        }
    }
}
