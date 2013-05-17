using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace TaskRunner.Tasks.Test
{
    [TestClass]
    public class DependencyJobRunnerImplTest
    {
        [TestMethod, Ignore]
        public void RunNextJob_Returns_Null_When_No_Jobs_To_Run()
        {
            // TODO: Throw an exception instead?
            var repository = new JobRepositoryStub();
            repository.JobTable = new List<JobRepositoryStub.JobRecord>();
            repository.JobHistoryTable = new List<JobRepositoryStub.JobHistoryRecord>();

            //var result = repository.GetNextJobToRun();

            //Assert.IsNull(result);
        }

        [TestMethod]
        public void RunNextJob_Runs_Job_With_No_Dependency_If_No_History_Found()
        {
            var repository = new JobRepositoryStub();
            var runner = new DefaultJobRunnerImpl<Job>(repository);

            var result = runner.RunNextJob();

            Assert.AreEqual(1, result.JobId);
        }

        /*

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



        /* -------------------- */

        [TestMethod]
        public void Execute_Returns_Proper_Number_Of_Results_For_No_Jobs()
        {
            var runner = GetJobRunner();
            var jobs = new List<DefaultJobImpl>();

            var results = new List<JobHistory>(runner.Execute(jobs));

            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void Execute_Returns_Proper_Number_Of_Results_For_Two_Jobs()
        {
            var runner = GetJobRunner();
            var jobs = GetJobs();

            var results = new List<JobHistory>(runner.Execute(jobs));

            Assert.AreEqual(jobs.Count, results.Count);
        }

        [TestMethod]
        public void Execute_Runs_Peer_Job_Even_If_Antecedent_Fails()
        {
            var runner = GetJobRunner();
            var jobs = GetJobsWithFailure();

            var results = new List<JobHistory>(runner.Execute(jobs));

            Assert.AreEqual(3, results.Count);
            Assert.IsTrue(results[0].Successful);
            Assert.IsFalse(results[1].Successful);
            Assert.IsTrue(results[2].Successful);
        }

        [TestMethod]
        public void Execute_Does_Not_Run_Dependent_Jobs_If_Parent_Fails()
        {
            var runner = GetJobRunner();
            var jobs = new[] {
                new FailingJobImpl(1, "job 1", 1),
                new DefaultJobImpl(2, "job 2", 1, 1)
            };

            var results = new List<JobHistory>(runner.Execute(jobs));

            Assert.AreEqual(1, results.Count);
        }

        protected DefaultJobRunnerImpl<Job> GetJobRunner()
        {
            var repo = new JobRepositoryStub();
            var seq = new DefaultJobSequencer<Job>();

            return new DefaultJobRunnerImpl<Job>(repo, seq);
        }

        protected IList<DefaultJobImpl> GetJobs()
        {
            var job1 = new DefaultJobImpl(1, "job 1", 1);
            var job2 = new DefaultJobImpl(2, "job 2", 1, 1);

            return new[] { job2, job1 };
        }

        protected IList<Job> GetJobsWithFailure()
        {
            var job1 = new DefaultJobImpl(1, "job 1", 1);
            var job2 = new FailingJobImpl(2, "job 2", 1, 1);
            var job3 = new DefaultJobImpl(3, "job 3", 1, 1);

            return new[] { job2, job3, job1 };
        }

        public class FailingJobImpl : DefaultJobImpl
        {
            public FailingJobImpl(int id, string name, int maxDurationMinutes, int? dependencyId = null)
                : base(id, name, maxDurationMinutes, dependencyId)
            {
            }

            protected override JobHistory GetResult()
            {
                var result = base.GetResult();
                result.Successful = false;
                return result;
            }
        }
    }
}
