using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskRunner.Tasks;

namespace PropertyManagement
{
    public static class JobFactory
    {
        public static Job GetJob(int id, string name, int maxDurationSeconds, int? dependencyId)
        {
            // TODO: Write a more robust implementation.
            switch (name.ToLower())
            {
                case "rent charges":
                    return new RentChargesJob(id, name, maxDurationSeconds, dependencyId);
                case "rent payments":
                    return new RentPaymentsJob(id, name, maxDurationSeconds, dependencyId);
                case "late fees":
                    return new LateFeesJob(id, name, maxDurationSeconds, dependencyId);
                case "bill customers for eft payments":
                    return new BillForEftPaymentsJob(id, name, maxDurationSeconds, dependencyId);
                default:
                    return new DefaultJob(id, name, maxDurationSeconds, dependencyId);
            }
        }
    }
}
