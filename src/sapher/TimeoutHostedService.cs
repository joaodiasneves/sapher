namespace Sapher
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Dtos;
    using Logger;
    using Logger.Extensions;
    using Microsoft.Extensions.Hosting;
    using Persistence;

    internal class TimeoutHostedService : IHostedService, IDisposable
    {
        private readonly ILogger logger;
        private readonly ISapherDataRepository repository;
        private readonly IInternalSapher sapher;
        private Timer timer;

        internal TimeoutHostedService(
            ILogger logger,
            ISapherDataRepository repository,
            IInternalSapher sapher)
        {
            this.logger = logger;
            this.repository = repository;
            this.sapher = sapher;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.Verbose("Timeout Background Service is starting.");

            timer = new Timer(
                async _ => await TimeoutStepInstances().ConfigureAwait(false),
                null,
                TimeSpan.FromMinutes(5), // TODO - Make this configurable
                TimeSpan.FromMinutes(30));

            logger.Verbose("Timeout Background Service started.");

            return Task.CompletedTask;
        }

        private async Task TimeoutStepInstances()
        {
            logger.Verbose("Timeout Background Service is running.");
            var instancesToTimeout = await this.repository.GetStepInstancesWaitingLonger(this.sapher.TimeoutInMinutes);

            var tasks = new List<Task>();

            foreach (var instance in instancesToTimeout)
            {
                instance.State = StepState.Timeout;
                tasks.Add(this.repository.Save(instance));
            }

            await Task.WhenAll(tasks);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.Verbose("Timeout Background Service is stopping.");

            timer?.Change(Timeout.Infinite, 0);

            logger.Verbose("Timeout Background Service stopped.");

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}