using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TaskRunner.Tasks.Test
{
    [TestClass]
    public class DefaultJobImplTest
    {
        [TestMethod]
        public void Job_Exceeding_MaxDuration_Fails()
        {
            var job = new LongRunningJob();

            var result = job.Execute();

            Assert.IsFalse(result.Successful);
        }

        public class LongRunningJob : DefaultJobImpl
        {
            public LongRunningJob()
                : base("long", 1)
            {

            }
            
            protected override void ExecuteJob()
            {
                Thread.Sleep(new TimeSpan(0, 1, 1));
            }
        }
    }
}
