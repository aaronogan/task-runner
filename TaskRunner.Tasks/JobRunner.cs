using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaskRunner.Tasks
{
    public interface JobRunner
    {
        void Execute();
    }

    public class DefaultJobRunnerImpl : JobRunner
    {
        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
