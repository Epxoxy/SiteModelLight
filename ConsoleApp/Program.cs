using SiteModelLight.Helpers;
using System;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            LoopingInvoker looping = null;
            looping = new LoopingInvoker(() =>
            {
                Console.WriteLine("Main invoked........");
                if (looping.Remain == 11)
                    throw new Exception("Error 11");
            }, 50, 20);
            looping.Invoked = () =>
            {
                if (looping.Remain == 21)
                    throw new Exception("Error 21");
                Console.WriteLine("Invoked [" + looping.Remain +"]");
            };
            looping.Started = () =>
            {
                Console.WriteLine("Started [" + looping.RepeatTimes + "]");
            };
            looping.Stopped = () =>
            {
                Console.WriteLine("Stopped [" + looping.Remain + "]");
            };
            looping.Start(e =>
            {
                Console.WriteLine("Exception ...." + Environment.NewLine + e.Message);
            });
            Console.ReadKey();
        }
    }
}
