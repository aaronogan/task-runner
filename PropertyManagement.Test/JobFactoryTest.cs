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
            var job = JobFactory.GetJob(2, "Rent Payments", 20 * 60, 1);

            Assert.IsInstanceOfType(job, typeof(RentPaymentsJob));
        }

        [TestMethod]
        public void GetJob_Returns_Proper_Implementation_For_Late_Fees()
        {
            var job = JobFactory.GetJob(3, "Late Fees", 60 * 60, 2);

            Assert.IsInstanceOfType(job, typeof(LateFeesJob));
        }

        [TestMethod]
        public void GetJob_Returns_Proper_Implementation_For_Eft_Payments()
        {
            var job = JobFactory.GetJob(4, "Bill customers for EFT payments", 30 * 60, 2);

            Assert.IsInstanceOfType(job, typeof(BillForEftPaymentsJob));
        }
    }
}
