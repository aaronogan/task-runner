using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaskRunner.Tasks
{
    public interface JobSequencer<T> where T : Job
    {
        Queue<T> ToSchedule { get; }
        Queue<T> GetSequencedJobs();
    }

    public class DependencyJobSequencer : JobSequencer<DependencyJobImpl>
    {
        public DependencyJobSequencer(IEnumerable<DependencyJobImpl> toSchedule)
        {
            ToSchedule = new Queue<DependencyJobImpl>(toSchedule);
        }

        public Queue<DependencyJobImpl> ToSchedule { get; protected set; }

        public Queue<DependencyJobImpl> GetSequencedJobs()
        {
            var sorted = new List<DependencyJobImpl>(ToSchedule).OrderBy(x => x.DependencyId);
            return new Queue<DependencyJobImpl>(sorted);
        }
    }
}
