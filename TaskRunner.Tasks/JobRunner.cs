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
        protected JobSequencer<T> Sequencer { get; set; }

        public DefaultJobRunnerImpl(JobRepository repository)
            : this (repository, new DefaultJobSequencer<T>())
        {
        }

        public DefaultJobRunnerImpl(JobRepository repository, JobSequencer<T> sequencer)
        {
            Repository = repository;
            Sequencer = sequencer;
        }

        public JobHistory RunNextJob()
        {
            var jobToRun = GetNextJobToRun();
            return jobToRun.Execute();
        }

        protected virtual Job GetNextJobToRun()
        {
            var allJobs = Repository.GetAllJobs();
            var sequenced = Sequencer.GetSequencedJobs((IEnumerable<T>)allJobs);

            while (sequenced.Any())
            {
                var job = sequenced.Dequeue();

                if (HasRunSuccessfullyToday(job.Id))
                {
                    continue;
                }

                if (!job.HasDependency())
                {
                    return job;
                }

                if (job.HasDependency()
                    && DependencyHasRunSuccessfullyToday(job.Id))
                {
                    return job;
                }
            }

            return null;
        }

        protected bool DependencyHasRunSuccessfullyToday(int jobId)
        {
            var dependencyId = Repository.GetJob(jobId).DependencyId;
            return dependencyId.HasValue && HasRunSuccessfullyToday(dependencyId.Value);
        }

        protected bool HasRunSuccessfullyToday(int jobId)
        {
            var jobHistory = Repository.GetJobHistory(jobId)
                .Where(x => x.ActivityTime.Date == DateTime.Today
                && x.Successful);

            return jobHistory.Any();
        }

        protected bool HasRunToday(int jobId)
        {
            var jobHistory = Repository.GetJobHistory(jobId)
                .Where(x => x.ActivityTime.Date == DateTime.Today);

            return jobHistory.Any();
        }

        [Obsolete]
        public IEnumerable<JobHistory> Execute(IEnumerable<T> jobs)
        {
            var queue = Sequencer.GetSequencedJobs(jobs);
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
