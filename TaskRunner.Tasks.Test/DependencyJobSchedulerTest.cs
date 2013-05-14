using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskRunner.Tasks;
using System.Collections.Generic;

namespace TaskRunner.Tasks.Test
{
    [TestClass]
    public class DependencyJobSchedulerTest
    {
        [TestMethod]
        public void GetScheduledJobs_Returns_Proper_Number_Of_Jobs()
        {
            var jobs = GetJobs();
            var scheduler = new DependencyJobScheduler(jobs);

            var scheduled = new List<Job>(scheduler.GetScheduledJobs());

            Assert.AreEqual(jobs.Count, scheduled.Count);
        }

        [TestMethod]
        public void GetScheduledJobs_Returns_Jobs_With_No_Dependencies_First()
        {
            var jobs = GetJobs();
            var scheduler = new DependencyJobScheduler(jobs);

            var scheduled = new List<Job>(scheduler.GetScheduledJobs());

            Assert.IsFalse(((DependencyJobImpl)scheduled[0]).DependencyId.HasValue);
        }

        protected IList<DependencyJobImpl> GetJobs()
        {
            var job1 = new DependencyJobImpl(1, "job 1", 1);
            var job2 = new DependencyJobImpl(2, "job 2", 1, 1);

            return new[] { job2, job1 };
        }
    }
}
