using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskRunner.Tasks;

namespace PropertyManagement
{
    public class BillingJobRepositoryStub : JobRepositoryStub
    {
        protected override Job ConvertToJob(JobRecord record)
        {
            return JobFactory.GetJob(record.Id, record.Name, record.MaxDurationSeconds, record.DependencyId);
        }
    }
}
