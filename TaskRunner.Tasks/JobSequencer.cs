using System.Collections.Generic;
using System.Linq;

namespace TaskRunner.Tasks
{
    public interface JobSequencer<T> where T : Job
    {
        Queue<T> GetSequencedJobs(IEnumerable<T> jobs);
    }

    public class DefaultJobSequencer<T> : JobSequencer<T> where T : Job
    {
        public Queue<T> GetSequencedJobs(IEnumerable<T> jobs)
        {
            var sorted = new List<T>(jobs).OrderBy(x => x.DependencyId);
            return new Queue<T>(sorted);
        }
    }
}
