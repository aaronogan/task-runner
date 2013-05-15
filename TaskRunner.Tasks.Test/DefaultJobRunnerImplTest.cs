using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace TaskRunner.Tasks.Test
{
    [TestClass]
    public class DependencyJobRunnerImplTest
    {
        [TestMethod]
        public void Execute_Returns_Proper_Number_Of_Results_For_No_Jobs()
        {
            var runner = new DependencyJobRunnerImpl<Job>();
            var jobs = new List<DefaultJobImpl>();

            var results = new List<JobResult>(runner.Execute(jobs));

            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void Execute_Returns_Proper_Number_Of_Results_For_Two_Jobs()
        {
            var runner = new DependencyJobRunnerImpl<Job>();
            var jobs = GetJobs();

            var results = new List<JobResult>(runner.Execute(jobs));

            Assert.AreEqual(jobs.Count, results.Count);
        }

        protected IList<DefaultJobImpl> GetJobs()
        {
            var job1 = new DefaultJobImpl(1, "job 1", 1);
            var job2 = new DefaultJobImpl(2, "job 2", 1, 1);

            return new[] { job2, job1 };
        }
    }
}
