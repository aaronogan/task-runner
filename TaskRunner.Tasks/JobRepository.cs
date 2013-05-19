﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaskRunner.Tasks
{
    public interface JobRepository
    {
        IEnumerable<Job> GetAllJobs();
        IEnumerable<Job> GetJobsWithoutDependencies();
        IEnumerable<Job> GetChildren(int id);
        Job GetJob(int id);
        IEnumerable<Job> GetPeers(int id);
        IEnumerable<JobHistory> GetAllHistory();
        IEnumerable<JobHistory> GetJobHistory(int id);
        void Save(JobHistory history);
    }

    public class JobRepositoryStub : JobRepository
    {
        public JobRepositoryStub()
            : this(JobRecord.DefaultRecords.ToList(), JobHistoryRecord.DefaultRecords.ToList())
        {
        }

        public JobRepositoryStub(IList<JobRecord> jobTable, IList<JobHistoryRecord> jobHistoryTable)
        {
            JobTable = jobTable;
            JobHistoryTable = jobHistoryTable;
        }

        public IList<JobRecord> JobTable { get; set; }
        public IList<JobHistoryRecord> JobHistoryTable { get; set; }

        public IEnumerable<Job> GetAllJobs()
        {
            return JobTable.Select(x => JobRecord.ConvertToJob(x));
        }

        public IEnumerable<Job> GetJobsWithoutDependencies()
        {
            return JobTable.Where(x => !x.DependencyId.HasValue).Select(x => JobRecord.ConvertToJob(x));
        }

        public IEnumerable<Job> GetChildren(int id)
        {
            return JobTable.Where(x => x.DependencyId.HasValue && x.DependencyId.Value == id).Select(x => JobRecord.ConvertToJob(x));
        }

        public Job GetJob(int id)
        {
            var job = JobTable.Single(x => x.Id == id);
            return JobRecord.ConvertToJob(job);
        }

        public IEnumerable<Job> GetPeers(int id)
        {
            var job = JobTable.Single(x => x.Id == id);

            if (job.DependencyId.HasValue)
            {
                var peers = JobTable.Where(x => x.DependencyId.HasValue
                    && x.DependencyId.Value == job.DependencyId.Value
                    && x.Id != id);

                return peers.Select(x => JobRecord.ConvertToJob(x));
            }

            return new List<Job>();
        }

        public IEnumerable<JobHistory> GetAllHistory()
        {
            return JobHistoryTable.Select(x => JobHistoryRecord.ConvertToHistory(x));
        }

        public IEnumerable<JobHistory> GetJobHistory(int id)
        {
            return JobHistoryTable.Where(x => x.JobId == id).Select(x => JobHistoryRecord.ConvertToHistory(x));
        }

        public void Save(JobHistory history)
        {
            JobHistoryTable.Add(new JobHistoryRecord
                {
                    JobId = history.JobId,
                    ActivityTime = history.ActivityTime,
                    Successful = history.Successful,
                    Error = history.Error
                });
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
                return new DefaultJob(record.Id, record.Name, record.MaxDurationMinutes, record.DependencyId);
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
