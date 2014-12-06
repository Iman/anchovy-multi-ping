using System;
using System.Threading;

namespace AnchovyMultiPing
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args == null) throw new ArgumentNullException("args");
            Console.WriteLine("--- Multi Thread ---");

            var waiter = new ManualResetEventSlim(false);

            //Dummy IP addresses provided, change them with real one.
            var pingData = new MultiPing(new[] {"69.000.00.00", "127.0.0.1", "10.11.12.15"}, waiter, 300);

            waiter.Wait();

            Console.WriteLine("Pings:");
            Console.WriteLine(pingData.GetPingInformation());

            Console.WriteLine("Server with lowest ping latency:");
            Console.WriteLine(pingData.GetIp());

            Console.ReadLine();
        }
    }
}