using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskRunner.Tasks;

namespace PropertyManagement
{
    public class RentChargesJob : DefaultJob
    {
        public RentChargesJob(int id)
            : this(id, "Rent Charges", 20 * 60, null)
        {
        }

        public RentChargesJob(int id, string name, int maxDurationSeconds, int? dependencyId)
            : base(id, name, maxDurationSeconds, dependencyId)
        {
        }

        protected override void ExecuteJob()
        {
            // TODO: Implementation specific to charging rent, such as API calls to a billing system.
            base.ExecuteJob();
        }

        protected override JobHistory GetResult()
        {
            // TODO: Implementation specific to charging rent, such as defining criteria for a successful run.
            return base.GetResult();
        }
    }
}
