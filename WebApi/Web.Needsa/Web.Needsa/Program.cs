using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace Web.Needsa
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //todo: must change the urls
            var urls = new [] { "http://localhost:80", "http://192.168.0.82:80" };
            //var urls = new[] { "http://localhost:80", "http://10.0.0.116:80" };

            var host = new WebHostBuilder()
                .UseUrls(urls)
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            host.Run();
        }
    }
}
