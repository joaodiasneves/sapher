namespace UsageSample
{
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Sapher.Configuration.Extensions;

    public static class Program
    {
        public static async Task Main()
        {
            var hostBuilder = new HostBuilder()
                 .ConfigureAppConfiguration((config) => config.SetBasePath(Directory.GetCurrentDirectory()))
                 .ConfigureServices(serviceCollection =>
                    serviceCollection
                        .AddSapher(sapherConfig => sapherConfig
                            .AddStep<TestHandleInput>("StepName", stepConfig => stepConfig
                                .AddResponseHandler<TestHandleSuccess>()
                                .AddResponseHandler<TestHandleCompensation>())
                            .AddLogger<>()
                            .AddPersistence<>())
                        .AddHostedService<TestService>())
                 .UseConsoleLifetime();

            await hostBuilder.Build().RunAsync().ConfigureAwait(false);
        }
    }
}