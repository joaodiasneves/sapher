namespace Sapher
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Configuration;
    using Dtos;
    using Exceptions;
    using global::Sapher.Logger.Extensions;
    using Logger;
    using Microsoft.Extensions.DependencyInjection;
    using Polly;

    public class Sapher : ISapher
    {
        private readonly ISapherConfiguration configuration;
        private ILogger logger;
        private IEnumerable<ISapherStep> steps = new List<ISapherStep>();
        private int maxRetryAttempts;
        private int retryIntervalMs;

        internal Sapher(ISapherConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void Init(IServiceProvider serviceProvider)
        {
            this.logger = serviceProvider.GetRequiredService<ILogger>();
            SetupSteps(serviceProvider);
            SetupRetryPolicy();
        }

        public async Task<DeliveryResult> DeliverMessage<T>(
            T message,
            MessageSlip messageSlip,
            string stepName = null)
            where T : class
        {
            var executionOutcome = await Policy
              .Handle<SapherException>()
              .Or<SapherConfigurationException>()
              .Or<Exception>()
              .WaitAndRetryAsync(
                this.maxRetryAttempts,
                _ => TimeSpan.FromMilliseconds(this.retryIntervalMs))
              .ExecuteAndCaptureAsync(() => this.ExecuteDelivery(message, messageSlip, stepName))
              .ConfigureAwait(false);

            var result = executionOutcome.Result;

            if (result == null)
            {
                var finalException = executionOutcome.FinalException;
                if (finalException.GetType() == typeof(SapherException))
                {
                    this.logger.Warning(finalException);
                }
                else
                {
                    this.logger.Error(finalException);
                }

                result = new DeliveryResult
                {
                    IsDeliveryFailed = true,
                    ErrorMessage = finalException.Message,
                    Exception = finalException
                };
            }

            return result;
        }

        internal async Task<DeliveryResult> ExecuteDelivery<T>(
            T message,
            MessageSlip messageSlip,
            string stepName = null)
            where T : class
        {
            var deliveryResult = new DeliveryResult();

            var affectedSteps = this.steps;

            if (!string.IsNullOrWhiteSpace(stepName))
            {
                affectedSteps = affectedSteps
                    .Where(s =>
                        string.Equals(
                            s.StepName,
                            stepName,
                            StringComparison.InvariantCultureIgnoreCase));
            }

            var tasks = new List<Task<StepResult>>();

            foreach (var step in affectedSteps)
            {
                tasks.Add(step.Deliver(message, messageSlip));
            }

            var stepResults = await Task.WhenAll(tasks).ConfigureAwait(false);
            deliveryResult.StepsExecuted = stepResults.Where(s => s != null).ToList();

            return deliveryResult;
        }

        internal void SetupRetryPolicy()
        {
            this.maxRetryAttempts = this.configuration.MaxRetryAttempts;
            this.retryIntervalMs = this.configuration.RetryIntervalMs;
        }

        internal void SetupSteps(IServiceProvider serviceProvider)
        {
            if (this.configuration.SapherSteps?.Any() == true)
            {
                this.steps = this.configuration.SapherSteps;

                foreach (var step in this.steps)
                {
                    step.Init(serviceProvider, this.logger);
                }
            }
            else
            {
                throw new SapherException("Trying to Use Sapher without defining any step"); // TODO - Improve exceptions
            }
        }
    }
}