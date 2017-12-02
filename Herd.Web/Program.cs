using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;

namespace Herd.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("Usage => dotnet run -- urls=http://localhost;http://url-1.com;http://url-2.com");
            }
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls(args[0].Split("=")[1])
                .Build();
    }
}