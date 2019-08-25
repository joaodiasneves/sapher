namespace Sapher
{
    using System;
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
                DoWork,
                null,
                TimeSpan.FromMinutes(5),
                TimeSpan.FromMinutes(30));

            logger.Verbose("Timeout Background Service started.");

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            logger.Verbose("Timeout Background Service is running.");

            bool selector(SapherStepData instance)
                => !instance.IsFinished
                && instance.IsExpectingResponses
                && (DateTime.UtcNow - instance.UpdatedOn).TotalMinutes > this.sapher.TimeoutInMinutes;

            this.repository.UpdateInstancesState(selector, Dtos.StepState.Timeout);
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