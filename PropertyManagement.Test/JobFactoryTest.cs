using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskRunner.Tasks;
using PropertyManagement;

namespace PropertyManagement.Test
{
    [TestClass]
    public class JobFactoryTest
    {
        [TestMethod]
        public void GetJob_Returns_Proper_Implementation_For_Rent_Charges()
        {
            var job = JobFactory.GetJob(1, "Rent Charges", 20, null);

            Assert.IsInstanceOfType(job, typeof(RentChargesJob));
        }
    }
}
