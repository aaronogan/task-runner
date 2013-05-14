﻿using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TaskRunner.Tasks.Test
{
    [TestClass]
    public class DefaultJobImplTest
    {
        [TestMethod]
        public void Job_Less_Than_MaxDuration_Succeeds()
        {
            var job = new DefinedTimeJob("short", 1, new TimeSpan(0, 0, 0));

            var result = job.Execute();

            Assert.IsTrue(result.Successful);
        }

        [TestMethod]
        public void Job_Exceeding_MaxDuration_Fails()
        {
            var job = new DefinedTimeJob("long", 1, new TimeSpan(0, 1, 1));

            var result = job.Execute();

            Assert.IsFalse(result.Successful);
        }

        public class DefinedTimeJob : DefaultJobImpl
        {
            private TimeSpan _executionTime;

            public DefinedTimeJob(string name, int maxDurationMinutes, TimeSpan executionTime)
                : base(name, maxDurationMinutes)
            {
                _executionTime = executionTime;
            }

            protected override void ExecuteJob()
            {
                Thread.Sleep(_executionTime);
            }
        }
    }
}