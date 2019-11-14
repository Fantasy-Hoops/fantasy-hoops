using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Sentry;

namespace fantasy_hoops
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (SentrySdk.Init("https://d29efee590ed4facbe9a287407a86eaa@sentry.io/1820879"))
            {
                CreateWebHostBuilder(args).Build().Run();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
