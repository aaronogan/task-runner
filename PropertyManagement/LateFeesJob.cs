using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskRunner.Tasks;

namespace PropertyManagement
{
    public class LateFeesJob : DefaultJob
    {
        public LateFeesJob(int id)
            : this(id, "Rent Charges", 60 * 60, null)
        {
        }

        public LateFeesJob(int id, string name, int maxDurationSeconds, int? dependencyId)
            : base(id, name, maxDurationSeconds, dependencyId)
        {
        }

        protected override void ExecuteJob()
        {
            // TODO: Implementation specific to late fees, such as API calls to a billing system.
            base.ExecuteJob();
        }

        protected override JobHistory GetResult()
        {
            // TODO: Implementation specific to late fees, such as defining criteria for a successful run.
            return base.GetResult();
        }
    }
}
