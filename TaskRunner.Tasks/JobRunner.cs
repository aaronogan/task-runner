using System.Collections.Generic;
using System.Linq;

namespace TaskRunner.Tasks
{
    public interface JobRunner<T> where T : Job
    {
        IEnumerable<JobResult> Execute(IEnumerable<T> jobs);
    }

    public class DependencyJobRunnerImpl<T> : JobRunner<T> where T : Job
    {
        private JobSequencer<T> _jobSequencer;

        public DependencyJobRunnerImpl()
            : this(new DependencyJobSequencer<T>())
        {
        }

        public DependencyJobRunnerImpl(JobSequencer<T> sequencer)
        {
            _jobSequencer = sequencer;
        }

        public IEnumerable<JobResult> Execute(IEnumerable<T> jobs)
        {
            var queue = _jobSequencer.GetSequencedJobs(jobs);
            var results = new List<JobResult>();

            while (queue.Any())
            {
                var previous = results.Any() ? results.Last() : null;
                bool continueProcessing = previous == null || previous.Successful;

                var jobToProcess = queue.Dequeue();
                var result = jobToProcess.Execute();
                results.Add(result);
            }

            return results;
        }
    }
}
