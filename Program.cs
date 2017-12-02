using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WebClient
{
    class Program
    {
        public const string defaultIpAddress = "127.0.0.1";
        public const int defaultPort = 10001;

        static void Main(string[] args)
        {
            var clientCfg = new ClientCfg() { IpAddressRaw = defaultIpAddress, Port = defaultPort };

            var result = Enumerable.Range(0, 100)
                .Select(x =>
                {
                    return Task.Run(async () =>
                    {
                        while (true)
                        {
                            using (var client = new Client(clientCfg))
                            {
                                await client.TestSending();
                            }
                        }
                    });
                })
                .ToArray();

            Task.WaitAll(result);
        }
    }
}
