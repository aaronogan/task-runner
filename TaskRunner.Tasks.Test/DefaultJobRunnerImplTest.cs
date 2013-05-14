using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskRunner.Tasks;
using System.Collections;
using System.Collections.Generic;

namespace TaskRunner.Tasks.Test
{
    [TestClass]
    public class DefaultJobRunnerImplTest
    {
        [TestMethod]
        public void Execute_Returns_Proper_Number_Of_Results_For_No_Jobs()
        {
            var runner = new DefaultJobRunnerImpl();
            var jobs = new List<Job>();

            var results = new List<JobResult>(runner.Execute(jobs));

            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void Execute_Return_Proper_Number_Of_Results_For_Two_Jobs()
        {
            var runner = new DefaultJobRunnerImpl();
            var jobs = GetJobs();

            var results = new List<JobResult>(runner.Execute(jobs));

            Assert.AreEqual(jobs.Count, results.Count);
        }

        protected IList<Job> GetJobs()
        {
            var job1 = new DefaultJobImpl(1, "job 1", 1);
            var job2 = new DefaultJobImpl(2, "job 2", 1);
            
            return new[] { job1, job2 };
        }
    }
}
