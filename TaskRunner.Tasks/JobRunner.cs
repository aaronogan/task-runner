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
        public DefaultJobRunnerImpl(JobRepository repository)
            : this (repository, new DefaultJobSequencer<T>())
        {
        }

        public DefaultJobRunnerImpl(JobRepository repository, JobSequencer<T> sequencer)
        {
            Repository = repository;
            Sequencer = sequencer;
        }

        protected JobRepository Repository { get; set; }
        protected JobSequencer<T> Sequencer { get; set; }

        public JobHistory RunNextJob()
        {
            var jobToRun = GetNextJobToRun();
            if (jobToRun == null) return null;
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
                    if (HasFailedToday(job.Id))
                    {
                        var peer = Repository.GetPeers(job.Id).FirstOrDefault();

                        if (peer != null)
                        {
                            return peer;
                        }
                    }

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

        protected bool HasFailedToday(int jobId)
        {
            var jobHistory = Repository.GetJobHistory(jobId)
                .Where(x => x.ActivityTime.Date == DateTime.Today
                && !x.Successful);

            return jobHistory.Any();
        }
    }
}
