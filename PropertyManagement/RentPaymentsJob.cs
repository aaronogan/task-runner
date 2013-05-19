using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskRunner.Tasks;

namespace PropertyManagement
{
    public class RentPaymentsJob : DefaultJob
    {
        public RentPaymentsJob(int id)
            : this(id, "Rent Payments", 20 * 60, 1)
        {
        }

        public RentPaymentsJob(int id, string name, int maxDurationSeconds, int? dependencyId)
            : base(id, name, maxDurationSeconds, dependencyId)
        {
        }

        protected override void ExecuteJob()
        {
            // TODO: Implementation specific to rent payments, such as API calls to a billing system.
            base.ExecuteJob();
        }

        protected override JobHistory GetResult()
        {
            // TODO: Implementation specific to rent payments, such as defining criteria for a successful run.
            return base.GetResult();
        }
    }
}
