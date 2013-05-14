using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace TaskRunner.Tasks.Test
{
    [TestClass]
    public class DependencyJobSequencerTest
    {
        [TestMethod]
        public void GetSequencedJobs_Returns_Proper_Number_Of_Jobs()
        {
            var jobs = GetJobs();
            var scheduler = new DependencyJobSequencer();

            var scheduled = new List<Job>(scheduler.GetSequencedJobs(jobs));

            Assert.AreEqual(jobs.Count, scheduled.Count);
        }

        [TestMethod]
        public void GetSequencedJobs_Returns_Jobs_With_No_Dependencies_First()
        {
            var jobs = GetJobs();
            var scheduler = new DependencyJobSequencer();

            var scheduled = new List<DependencyJobImpl>(scheduler.GetSequencedJobs(jobs));

            Assert.IsFalse(scheduled[0].DependencyId.HasValue);
            Assert.IsTrue(scheduled[1].DependencyId.HasValue);
            Assert.AreEqual(1, scheduled[1].DependencyId.Value);
        }

        protected IList<DependencyJobImpl> GetJobs()
        {
            var job1 = new DependencyJobImpl(1, "job 1", 1);
            var job2 = new DependencyJobImpl(2, "job 2", 1, 1);

            return new[] { job2, job1 };
        }
    }
}
