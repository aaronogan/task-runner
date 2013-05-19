using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskRunner.Tasks
{
    public interface JobRunner
    {
        Job GetNext();
        JobHistory RunNextJob();
    }

    public class DefaultJobRunner : JobRunner
    {
        public DefaultJobRunner(JobRepository repository)
        {
            Repository = repository;
        }

        protected JobRepository Repository { get; set; }

        public JobHistory RunNextJob()
        {
            var jobToRun = GetNext();
            if (jobToRun == null) return null;

            var result = jobToRun.Execute();
            Repository.Save(result);

            return result;
        }

        protected IEnumerable<Job> GetChildren(Job job)
        {
            return Repository.GetChildren(job.Id);
        }

        public virtual Job GetNext()
        {
            var rootJobs = Repository.GetJobsWithoutDependencies();

            foreach (var job in rootJobs)
            {
                var node = Traverse(job);

                if (node == null)
                {
                    continue;
                }

                return node;
            }
            
            return null;
        }

        protected Job Traverse(Job start)
        {
            if (JobShouldRun(start))
            {
                return start;
            }

            var children = Repository.GetChildren(start.Id);

            foreach (var child in children)
            {
                var current = Traverse(child);

                if (current == null)
                {
                    continue;
                }

                return current;
            }

            return null;
        }

        protected bool JobShouldRun(Job job)
        {
            if (HasRunSuccessfullyToday(job.Id))
            {
                return false;
            }

            if (HasFailedToday(job.Id))
            {
                if (HasPeers(job.Id))
                {
                    return false;
                }

                return true;
            }

            if (!job.HasDependency())
            {
                return true;
            }

            if (job.HasDependency()
                && DependencyHasRunSuccessfullyToday(job.Id))
            {
                return true;
            }

            return false;
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

        protected bool HasPeers(int jobId)
        {
            var peers = Repository.GetPeers(jobId);
            return peers.Any();
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
