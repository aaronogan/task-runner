using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskRunner.Tasks;

namespace PropertyManagement
{
    public class JobFactory
    {
        public static Job GetJob(int id, string name, int maxDurationSeconds, int? dependencyId)
        {
            switch (name.ToLower())
            {
                case "rent charges":
                    return new RentChargesJob(id, name, maxDurationSeconds, dependencyId);
                default:
                    return new DefaultJob(id, name, maxDurationSeconds, dependencyId);
            }

            throw new NotImplementedException();
        }
    }
}
