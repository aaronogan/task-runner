using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskRunner.Tasks;

namespace PropertyManagement
{
    public class BillForEftPaymentsJob : DefaultJob
    {
        public BillForEftPaymentsJob(int id)
            : this(id, "Rent Charges", 20 * 30, null)
        {
        }

        public BillForEftPaymentsJob(int id, string name, int maxDurationSeconds, int? dependencyId)
            : base(id, name, maxDurationSeconds, dependencyId)
        {
        }

        protected override void ExecuteJob()
        {
            // TODO: Implementation specific to billing for EFT payments, such as API calls to a billing system.
            base.ExecuteJob();
        }

        protected override JobHistory GetResult()
        {
            // TODO: Implementation specific to billing for EFT payments, such as defining criteria for a successful run.
            return base.GetResult();
        }
    }
}
