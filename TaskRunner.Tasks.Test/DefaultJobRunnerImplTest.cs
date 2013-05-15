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
            var runner = GetJobRunner();
            var jobs = new List<DependencyJobImpl>();

            var results = new List<JobResult>(runner.Execute(jobs));

            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void Execute_Return_Proper_Number_Of_Results_For_Two_Jobs()
        {
            var runner = GetJobRunner();
            var jobs = GetJobs();

            var results = new List<JobResult>(runner.Execute(jobs));

            Assert.AreEqual(jobs.Count, results.Count);
        }

        protected DependencyJobRunnerImpl<DependencyJobImpl> GetJobRunner()
        {
            var sequencer = new DependencyJobSequencer();
            return new DependencyJobRunnerImpl<DependencyJobImpl>(sequencer);
        }

        protected IList<DependencyJobImpl> GetJobs()
        {
            var job1 = new DependencyJobImpl(1, "job 1", 1);
            var job2 = new DependencyJobImpl(2, "job 2", 1, 1);

            return new[] { job1, job2 };
        }
    }
}
