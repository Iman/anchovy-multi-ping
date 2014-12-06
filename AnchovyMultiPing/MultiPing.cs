using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace AnchovyMultiPing
{
    internal sealed class MultiPing : AbstractMultiPing
    {
        private int Timeout { get; set; }
        private string[] Hosts { get; set; }
        private int _count;
        private ManualResetEventSlim Waiter { get; set; }
        private readonly byte[] _buffer = Encoding.ASCII.GetBytes("aaa");
        private Dictionary<string, long> _table = new Dictionary<string, long>();

        private class Parameters
        {
            public String Host { get; set; }
            public ManualResetEventSlim Event { get; set; }
        }

        public MultiPing(string[] hosts, ManualResetEventSlim waiter, int timeout = 12000)
        {
            Hosts = hosts;
            Waiter = waiter;
            Timeout = timeout;
            RequestTime();
        }

        public override IMultiPings RequestTime()
        {
            try
            {
                _count = 0;
                _table = new Dictionary<string, long>();
                foreach (string host in Hosts)
                {
                    using (var pingSender = new Ping())
                    {
                        pingSender.PingCompleted += PingCompletedCallback;
                        var options = new PingOptions(64, true);
                        try
                        {
                            pingSender.SendAsync(host, Timeout, _buffer, options,
                                                 new Parameters {Host = host, Event = Waiter});
                        }
                        catch
                        {
                            _count += 1;
                            if (_count == Hosts.Length)
                                Waiter.Set();
                        }
                    }
                }
            }
            catch (MultiPingException ex)
            {
                Console.Write("RequestTime Exception");
                Console.ReadKey();
            }
            return this;
        }

        //[MethodImpl(MethodImplOptions.Synchronized)] leaving this in favour of the operating system scheduler for better performance.
        private void PingCompletedCallback(object sender, PingCompletedEventArgs e)
        {
            try
            {
                _count += 1;

                PingReply reply = e.Reply;

                if (reply != null && reply.Address != null && reply.Address.ToString() != "0.0.0.0")
                {
                    if (_count > 0)
                    {
                        try
                        {
                            if (!_table.ContainsKey(reply.Address.ToString()))
                                _table.Add(((Parameters) e.UserState).Host, reply.RoundtripTime);
                        }
                        catch (NullReferenceException ex)
                        {
                            // catch null exception
                            throw new MultiPingException("Ping round trip time is null");
                        }
                    }
                }

                if (_count == Hosts.Length)
                {
                    ((Parameters) e.UserState).Event.Set();
                }


                if (e.Error != null)
                {
                    Console.WriteLine("Ping failed:");
                    Console.WriteLine(e.Error.ToString());
                    ((ManualResetEventSlim) e.UserState).Set();
                }
            }
            catch (MultiPingException ex)
            {
                Console.Write("Exception");
                Console.ReadKey();
            }
        }

        public override String GetPingInformation()
        {
            var build = new StringBuilder();
            foreach (string host in _table.Keys)
            {
                build.AppendLine(string.Format("{0} : {1}", host, _table[host]));
            }
            return build.ToString();
        }

        public override String GetIp()
        {
            string ip = "";
            long time = -1L;
            foreach (string host in _table.Keys)
            {
                long roundTime = _table[host];
                if ((time == -1L) || (roundTime >= 0 && roundTime < time))
                {
                    time = roundTime;
                    ip = host;
                }
            }
            return ip;
        }
    }
}