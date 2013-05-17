using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskRunner.Tasks
{
    public interface JobRunner<T> where T : Job
    {
        JobResult RunNextJob();
        [Obsolete]
        IEnumerable<JobResult> Execute(IEnumerable<T> jobs);
    }

    public class DefaultJobRunnerImpl<T> : JobRunner<T> where T : Job
    {
        protected JobRepository Repository { get; set; }
        private JobSequencer<T> _jobSequencer;

        public DefaultJobRunnerImpl(JobRepository repository, JobSequencer<T> sequencer)
        {
            Repository = repository;
            _jobSequencer = sequencer;
        }

        public JobResult RunNextJob()
        {
            throw new NotImplementedException();
        }

        [Obsolete]
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
