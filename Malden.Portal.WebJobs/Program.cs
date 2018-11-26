using Malden.Portal.BLL;
using Microsoft.WindowsAzure.Jobs;
using Ninject;
using System;
using System.Collections.Generic;

namespace WebJobs
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new JobHost();
            host.Call(typeof(Program).GetMethod("ManualTrigger"));
        }

        [NoAutomaticTrigger]
        public static void ManualTrigger([Table("log")] IDictionary<Tuple<string, string>, object> data)
        {
            var kernel = new StandardKernel();
            var depResolver = new WebjobDependencyResolver(kernel);
            depResolver.RegisterServices(kernel);
            var userLogic = kernel.Get<IUserLogic>();

            userLogic.DeleteInactiveUsers((int) Malden.Portal.BLL.User.UserType.Customer);

            var date = DateTime.Now;
            data.Add(new Tuple<string, string>(date.Year.ToString(), date.Month.ToString()), date);
            Console.WriteLine("Updated!!!");
        }

    }
}
