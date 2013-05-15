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
                var jobToProcess = queue.Dequeue();

                var parentJob = jobToProcess.HasDependency() ?
                    results.SingleOrDefault(x => x.JobId == jobToProcess.DependencyId.Value) :
                    null;

                bool processJob = parentJob == null || parentJob.Successful;

                if (processJob)
                {
                    var result = jobToProcess.Execute();
                    results.Add(result);
                }
            }

            return results;
        }
    }
}
