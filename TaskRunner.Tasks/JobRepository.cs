using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaskRunner.Tasks
{
    public interface JobRepository
    {
        Job GetNextJobToRun();
    }

    public class JobRepositoryStub : JobRepository
    {
        public IList<JobRecord> JobTable { get; set; }
        public IList<JobHistory> JobHistoryTable { get; set; }

        public JobRepositoryStub()
            : this(JobRecord.DefaultRecords, JobHistory.DefaultRecords)
        {
        }

        public JobRepositoryStub(IList<JobRecord> jobTable, IList<JobHistory> jobHistoryTable)
        {
            JobTable = jobTable;
            JobHistoryTable = jobHistoryTable;
        }

        public Job GetNextJobToRun()
        {
            var orderedJobs = JobTable.OrderBy(x => x.Id).OrderBy(x => x.DependencyId);

            foreach (var job in orderedJobs)
            {
                if (HasRunSuccessfullyToday(job.Id))
                {
                    continue;
                }

                if (!job.DependencyId.HasValue
                    || DependencyHasRunSuccessfullyToday(job.Id))
                {
                    return JobRecord.ConvertToJob(job);
                }
            }

            return null;
        }

        protected bool DependencyHasRunSuccessfullyToday(int jobId)
        {
            var dependencyId = JobTable.Single(x => x.Id == jobId).DependencyId;
            return dependencyId.HasValue && HasRunSuccessfullyToday(dependencyId.Value);
        }

        protected bool HasRunSuccessfullyToday(int jobId)
        {
            var jobHistory = JobHistoryTable.Where(x => x.JobId == jobId
                && x.ActivityTime.Date == DateTime.Today
                && x.Successful);

            return jobHistory.Any();
        }

        protected bool HasRunToday(int jobId)
        {
            var jobHistory = JobHistoryTable.Where(x => x.JobId == jobId
                && x.ActivityTime.Date == DateTime.Today);

            return jobHistory.Any();
        }

        public class JobRecord
        {
            public int Id { get; set; }
            public int? DependencyId { get; set; }
            public string Name { get; set; }
            public int MaxDurationMinutes { get; set; }

            public static IList<JobRecord> DefaultRecords = new[]
            {
                new JobRecord
                {
                    Id = 1,
                    Name = "Rent Charges",
                    MaxDurationMinutes = 20
                },
                new JobRecord
                {
                    Id = 2,
                    DependencyId = 1,
                    Name = "Rent Payments",
                    MaxDurationMinutes = 20
                },
                new JobRecord
                {
                    Id = 3,
                    DependencyId = 2,
                    Name = "Late Fees",
                    MaxDurationMinutes = 60,
                },
                new JobRecord
                {
                    Id = 4,
                    DependencyId = 2,
                    Name = "Bill customers for EFT payments",
                    MaxDurationMinutes = 30
                }
            };

            public static Job ConvertToJob(JobRecord record)
            {
                if (record == null) return null;
                return new DefaultJobImpl(record.Id, record.Name, record.MaxDurationMinutes, record.DependencyId);
            }
        }

        public class JobHistory
        {
            public int JobId { get; set; }
            public DateTime ActivityTime { get; set; }
            public bool Successful { get; set; }
            public string Error { get; set; }

            public static IList<JobHistory> DefaultRecords = new List<JobHistory>();
        }
    }
}
