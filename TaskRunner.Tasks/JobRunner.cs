using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskRunner.Tasks
{
    public interface JobRunner<T> where T : Job
    {
        JobHistory RunNextJob();
    }

    public class DefaultJobRunnerImpl<T> : JobRunner<T> where T : Job
    {
        protected JobRepository Repository { get; set; }
        protected JobSequencer<T> JobSequencer { get; set; }

        public DefaultJobRunnerImpl(JobRepository repository, JobSequencer<T> sequencer)
        {
            Repository = repository;
            JobSequencer = sequencer;
        }

        public JobHistory RunNextJob()
        {
            throw new NotImplementedException();
        }

        [Obsolete]
        public IEnumerable<JobHistory> Execute(IEnumerable<T> jobs)
        {
            var queue = JobSequencer.GetSequencedJobs(jobs);
            var results = new List<JobHistory>();

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
