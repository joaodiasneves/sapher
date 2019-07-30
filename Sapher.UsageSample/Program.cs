namespace UsageSample
{
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Sapher.Extensions;

    public static class Program
    {
        public static async Task Main()
        {
            var hostBuilder = new HostBuilder()
                 .ConfigureAppConfiguration((config) => config.SetBasePath(Directory.GetCurrentDirectory()))
                 .ConfigureServices(serviceCollection =>
                    serviceCollection
                        .AddSapher(sapherConfig => sapherConfig
                                .AddStep(
                                    "StepName",
                                    typeof(TestHandleInput),
                                    stepConfig => stepConfig
                                        .AddResponseHandler(typeof(TestHandleSuccess))
                                        .AddResponseHandler(typeof(TestHandleCompensation))))
                        .AddHostedService<TestService>())
                 .UseConsoleLifetime();

            await hostBuilder.Build().RunAsync().ConfigureAwait(false);
        }
    }
}