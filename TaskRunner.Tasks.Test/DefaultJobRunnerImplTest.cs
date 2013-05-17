using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace TaskRunner.Tasks.Test
{
    [TestClass]
    public class DependencyJobRunnerImplTest
    {
        [TestMethod]
        public void GetNext_Returns_Null_When_No_Jobs_To_Run()
        {
            var repository = new JobRepositoryStub();
            repository.JobTable = new List<JobRepositoryStub.JobRecord>();
            repository.JobHistoryTable = new List<JobRepositoryStub.JobHistoryRecord>();

            var runner = new DefaultJobRunnerImpl<Job>(repository);

            var result = runner.GetNext();

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetNext_Returns_Job_With_No_Dependency_If_No_History_Found()
        {
            var repository = new JobRepositoryStub();
            var runner = new DefaultJobRunnerImpl<Job>(repository);

            var result = runner.GetNext();

            Assert.AreEqual(1, result.Id);
        }

        [TestMethod]
        public void GetNext_Returns_Null_If_All_Jobs_Have_Run_Today()
        {
            var repository = new JobRepositoryStub();
            repository.JobTable = new[]
                {
                    new JobRepositoryStub.JobRecord
                    {
                        Id = 1,
                        DependencyId = null,
                        MaxDurationMinutes = 1,

                    }
                };
            repository.JobHistoryTable = new[]
                {
                    new JobRepositoryStub.JobHistoryRecord
                    {
                        JobId = 1,
                        ActivityTime = DateTime.Now.AddMinutes(-1),
                        Successful = true
                    }
                };

            var runner = new DefaultJobRunnerImpl<Job>(repository);

            var result = runner.GetNext();

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetNext_Returns_First_Job_With_No_Dependency_If_History_Found_For_First()
        {
            var repository = new JobRepositoryStub();
            repository.JobTable = new[]
                {
                    new JobRepositoryStub.JobRecord
                    {
                        Id = 1,
                        DependencyId = null,
                        MaxDurationMinutes = 1,

                    },
                    new JobRepositoryStub.JobRecord
                    {
                        Id = 2,
                        DependencyId = null,
                        MaxDurationMinutes = 1,
                    }
                };
            repository.JobHistoryTable = new[]
                {
                    new JobRepositoryStub.JobHistoryRecord
                    {
                        JobId = 1,
                        ActivityTime = DateTime.Now.AddMinutes(-1),
                        Successful = true
                    }
                };

            var runner = new DefaultJobRunnerImpl<Job>(repository);

            var result = runner.GetNext();

            Assert.AreEqual(2, result.Id);
        }

        [TestMethod]
        public void GetNext_Returns_First_Failed_Job()
        {
            var repository = new JobRepositoryStub();

            repository.JobHistoryTable.Add(
                new JobRepositoryStub.JobHistoryRecord
                {
                    JobId = 1,
                    ActivityTime = DateTime.Now.AddMinutes(-2),
                    Successful = true,
                    Error = string.Empty
                });
            repository.JobHistoryTable.Add(
                new JobRepositoryStub.JobHistoryRecord
                {
                    JobId = 2,
                    ActivityTime = DateTime.Now.AddMinutes(-1),
                    Successful = false,
                    Error = "Error"
                });

            var runner = new DefaultJobRunnerImpl<Job>(repository);

            var result = runner.GetNext();

            Assert.AreEqual(2, result.Id);
        }

        [TestMethod]
        public void GetNext_Retruns_Next_Peer_Level_Job_When_First_Peer_Has_Failed()
        {
            var repository = new JobRepositoryStub();

            repository.JobHistoryTable.Add(
                new JobRepositoryStub.JobHistoryRecord
                {
                    JobId = 1,
                    ActivityTime = DateTime.Now.AddMinutes(-3),
                    Successful = true,
                    Error = string.Empty
                });

            repository.JobHistoryTable.Add(
                new JobRepositoryStub.JobHistoryRecord
                {
                    JobId = 2,
                    ActivityTime = DateTime.Now.AddMinutes(-2),
                    Successful = true,
                    Error = string.Empty
                });

            repository.JobHistoryTable.Add(
                new JobRepositoryStub.JobHistoryRecord
                {
                    JobId = 3,
                    ActivityTime = DateTime.Now.AddMinutes(-1),
                    Successful = false,
                    Error = "Error"
                });

            var runner = new DefaultJobRunnerImpl<Job>(repository);

            var result = runner.GetNext();

            Assert.AreEqual(4, result.Id);
        }

        /*

        [TestMethod, Ignore]
        public void RunNextJob_Will_Not_Run_Failed_Peer_Level_Job_Again()
        {
            var repository = new JobRepositoryStub();

            repository.JobHistoryTable.Add(
                new JobRepositoryStub.JobHistoryRecord
                {
                    JobId = 1,
                    ActivityTime = DateTime.Now.AddMinutes(-3),
                    Successful = true,
                    Error = string.Empty
                });

            repository.JobHistoryTable.Add(
                new JobRepositoryStub.JobHistoryRecord
                {
                    JobId = 2,
                    ActivityTime = DateTime.Now.AddMinutes(-2),
                    Successful = true,
                    Error = string.Empty
                });

            repository.JobHistoryTable.Add(
                new JobRepositoryStub.JobHistoryRecord
                {
                    JobId = 3,
                    ActivityTime = DateTime.Now.AddMinutes(-1),
                    Successful = false,
                    Error = "Error"
                });

            repository.JobHistoryTable.Add(
                new JobRepositoryStub.JobHistoryRecord
                {
                    JobId = 4,
                    ActivityTime = DateTime.Now.AddMinutes(-1),
                    Successful = false,
                    Error = "Error"
                });

            var runner = new DefaultJobRunnerImpl<Job>(repository);

            var result = runner.RunNextJob();

            Assert.IsNull(result);
        }
         */
    }
}
