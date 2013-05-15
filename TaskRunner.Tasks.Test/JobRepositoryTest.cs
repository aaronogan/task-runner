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
            var jobs = new List<JobRepositoryStub.JobRecord>();
            var jobHistory = new List<JobRepositoryStub.JobHistory>();

            var repository = new JobRepositoryStub(jobs, jobHistory);

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
            var jobs = new[]
                {
                    new JobRepositoryStub.JobRecord
                    {
                        Id = 1,
                        DependencyId = null,
                        MaxDurationMinutes = 1,

                    }
                };

            var jobHistory = new[]
                {
                    new JobRepositoryStub.JobHistory
                    {
                        JobId = 1,
                        ActivityTime = DateTime.Now.AddMinutes(-1)
                    }
                };

            var repository = new JobRepositoryStub(jobs, jobHistory);

            var result = repository.GetNextJobToRun();

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetNextJobToRun_Returns_First_Job_With_No_Dependency_If_History_Found_For_First()
        {
            var jobs = new[]
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

            var jobHistory = new[]
                {
                    new JobRepositoryStub.JobHistory
                    {
                        JobId = 1,
                        ActivityTime = DateTime.Now.AddMinutes(-1)
                    }
                };

            var repository = new JobRepositoryStub(jobs, jobHistory);

            var result = repository.GetNextJobToRun();

            Assert.IsFalse(result.HasDependency());
            Assert.AreEqual(2, result.Id);
        }
    }
}
