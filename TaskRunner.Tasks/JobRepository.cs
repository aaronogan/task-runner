using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaskRunner.Tasks
{
    public interface JobRepository
    {
        IEnumerable<Job> GetAllJobs();
        IEnumerable<JobHistory> GetAllHistory();
        
        //[Obsolete]
        //Job GetNextJobToRun();
    }

    public class JobRepositoryStub : JobRepository
    {
        public IList<JobRecord> JobTable { get; set; }
        public IList<JobHistoryRecord> JobHistoryTable { get; set; }
        //protected JobSequencer<Job> Sequencer { get; set; }

        public JobRepositoryStub()
            : this(new DefaultJobSequencer<Job>(), JobRecord.DefaultRecords, JobHistoryRecord.DefaultRecords)
        {
        }

        public JobRepositoryStub(JobSequencer<Job> jobSequencer, IList<JobRecord> jobTable, IList<JobHistoryRecord> jobHistoryTable)
        {
            //Sequencer = jobSequencer;
            JobTable = jobTable;
            JobHistoryTable = jobHistoryTable;
        }

        public IEnumerable<Job> GetAllJobs()
        {
            return JobTable.Select(x => JobRecord.ConvertToJob(x));
        }

        public IEnumerable<JobHistory> GetAllHistory()
        {
            return JobHistoryTable.Select(x => JobHistoryRecord.ConvertToHistory(x));
        }

        [Obsolete("This needs to be moved into the JobRunner implementation.")]
        public Job GetNextJobToRun()
        {
            throw new NotImplementedException();

            //var allJobs = JobTable.Select(x => JobRecord.ConvertToJob(x));
            //var sequenced = Sequencer.GetSequencedJobs(allJobs);

            //while (sequenced.Any())
            //{
            //    var job = sequenced.Dequeue();

            //    if (HasRunSuccessfullyToday(job.Id))
            //    {
            //        continue;
            //    }

            //    if (!job.HasDependency())
            //    {
            //        return job;
            //    }

            //    if (job.HasDependency()
            //        && DependencyHasRunSuccessfullyToday(job.Id))
            //    {
            //        return job;
            //    }
            //}

            //return null;
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

        public class JobHistoryRecord
        {
            public int JobId { get; set; }
            public DateTime ActivityTime { get; set; }
            public bool Successful { get; set; }
            public string Error { get; set; }

            public static IList<JobHistoryRecord> DefaultRecords = new List<JobHistoryRecord>();

            public static JobHistory ConvertToHistory(JobHistoryRecord record)
            {
                if (record == null) return null;
                return new JobHistory
                {
                    JobId = record.JobId,
                    ActivityTime = record.ActivityTime,
                    Successful = record.Successful,
                    Error = record.Error
                };
            }
        }
    }
}
