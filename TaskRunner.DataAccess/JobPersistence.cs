using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskRunner.Tasks;

namespace TaskRunner.DataAccess
{
    public interface JobPersistence
    {
        void Save(JobResult result);
    }
}
