namespace TestConsole
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Sapher;

    public class TestService : IHostedService
    {
        private readonly ISapher sapher;

        public TestService(ISapher sapher)
        {
            this.sapher = sapher;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // TODO - Add Logs
            this.sapher.Init();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}