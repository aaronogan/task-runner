using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskRunner.Tasks;
using PropertyManagement;

namespace PropertyManagement.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var repository = new BillingJobRepositoryStub();
            var runner = new DefaultJobRunner(repository);

            var result = runner.RunNextJob();

            Console.WriteLine(result.ToString());

            Console.ReadKey();
        }
    }
}
