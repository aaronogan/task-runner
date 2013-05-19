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
            var job = JobFactory.GetJob(1, "Rent Charges", 20 * 60, null);

            Assert.IsInstanceOfType(job, typeof(RentChargesJob));
        }

        [TestMethod]
        public void GetJob_Returns_Proper_Implementation_For_Rent_Payments()
        {
            var job = JobFactory.GetJob(1, "Rent Payments", 20 * 60, 1);

            Assert.IsInstanceOfType(job, typeof(RentPaymentsJob));
        }
    }
}
