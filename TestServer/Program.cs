using SuperSocket.SocketBase;
using SuperSocket.SocketEngine;
using System;
namespace TestServer
{
    class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Press any key to start the server!");
            Console.ReadKey();
            Console.WriteLine();
            IBootstrap bootstrap = BootstrapFactory.CreateBootstrap();
            if (!bootstrap.Initialize())
            {
                Console.WriteLine("Failed to initialize!");
                Console.ReadKey();
            }
            else
            {
                StartResult result = bootstrap.Start();
                Console.WriteLine("Start result: {0}!", result);
                if (result == StartResult.Failed)
                {
                    Console.WriteLine("Failed to start!");
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("Press key 'q' to stop it!");
                    while (Console.ReadKey().KeyChar != 'q')
                    {
                        Console.WriteLine();
                    }
                    Console.WriteLine();
                    bootstrap.Stop();
                    foreach (var item in bootstrap.AppServers)
                    {
                    }
                    Console.WriteLine("The server was stopped!");
                }
            }
        }
    }
}
