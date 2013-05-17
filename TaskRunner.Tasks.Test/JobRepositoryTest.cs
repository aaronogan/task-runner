using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace TaskRunner.Tasks.Test
{
    [TestClass]
    public class JobRepositoryStubTest
    {
        [TestMethod]
        public void GetNextJobToRun_Returns_Null_When_No_Jobs_To_Run()
        {
            var repository = new JobRepositoryStub();
            repository.JobTable = new List<JobRepositoryStub.JobRecord>();
            repository.JobHistoryTable = new List<JobRepositoryStub.JobHistoryRecord>();

            var result = repository.GetNextJobToRun();

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetNextJobToRun_Returns_Job_With_No_Dependency_If_No_History_Found()
        {
            var repository = new JobRepositoryStub();

            var result = repository.GetNextJobToRun();

            Assert.IsFalse(result.HasDependency());
        }

        [TestMethod]
        public void GetNextJobToRun_Returns_Null_If_All_Jobs_Have_Run_Today()
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

            var result = repository.GetNextJobToRun();

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetNextJobToRun_Returns_First_Job_With_No_Dependency_If_History_Found_For_First()
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

            var result = repository.GetNextJobToRun();

            Assert.IsFalse(result.HasDependency());
            Assert.AreEqual(2, result.Id);
        }

        [TestMethod]
        public void GetNextJobToRun_Returns_First_Failed_Job()
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

            var result = repository.GetNextJobToRun();

            Assert.AreEqual(2, result.Id);
        }

        [TestMethod, Ignore]
        public void GetNextJobToRun_Returns_Next_Peer_Level_Job_When_First_Peer_Has_Failed()
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

            var result = repository.GetNextJobToRun();

            Assert.AreEqual(4, result.Id);
        }

        protected JobRepositoryStub GetRepository()
        {
            return new JobRepositoryStub();
        }
    }
}
