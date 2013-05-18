using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskRunner.Tasks;

namespace TaskRunner.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var repository = new JobRepositoryStub();
            var runner = new DefaultJobRunnerImpl<Job>(repository);

            var result = runner.RunNextJob();

            Console.WriteLine(result.ToString());

            Console.ReadKey();
        }
    }
}
