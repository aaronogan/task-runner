using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaskRunner.Tasks
{
    public interface JobRepository
    {
        Job GetNextJobToRun();
        void SaveJobHistory(JobResult history);
    }

    public class JobRepositoryStub : JobRepository
    {
        public IList<JobRecord> JobTable { get; set; }
        public IList<JobHistory> JobHistoryTable { get; set; }

        public JobRepositoryStub()
        {
            JobTable = JobRecord.DefaultRecords;
            JobHistoryTable = JobHistory.DefaultRecords;
        }

        public Job GetNextJobToRun()
        {
            throw new NotImplementedException();
        }

        public void SaveJobHistory(JobResult history)
        {
            throw new NotImplementedException();
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
        }

        public class JobHistory
        {
            public int JobId { get; set; }
            public DateTime ActivityTime { get; set; }
            public string Error { get; set; }

            public static IList<JobHistory> DefaultRecords = new List<JobHistory>();
        }
    }
}
