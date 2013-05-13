using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskRunner.Tasks;

namespace TaskRunner.DataAccess
{
    public class JobPersistenceStub : JobPersistence
    {
        public void Save(JobResult result)
        {
            throw new NotImplementedException();
        }
    }
}
