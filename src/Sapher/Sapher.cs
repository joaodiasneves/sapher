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
            var deliveryResult = new DeliveryResult();
            var retryCount = 0;
            RetryExecution:
            try
            {
                await this.ExecuteDelivery(message, messageSlip, deliveryResult, stepName).ConfigureAwait(false);
            }
            catch(SapherException sapherException)
            {
                this.logger.Warning(sapherException);
                deliveryResult.IsDeliveryFailed = true;
                deliveryResult.ErrorMessage = sapherException.Message;
                deliveryResult.Exception = sapherException;
            }
            catch (Exception exception)
            {
                this.logger.Error(exception);
                deliveryResult.IsDeliveryFailed = true;
                deliveryResult.ErrorMessage = exception.Message;
                deliveryResult.Exception = exception;
            }

            retryCount++;

            if (deliveryResult.IsDeliveryFailed && retryCount < this.maxRetryAttempts)
            {
                await Task.Delay(this.retryIntervalMs).ConfigureAwait(false);
                goto RetryExecution; // TODO Improve retry mechanism and do retry tests
            }

            return deliveryResult;
        }

        internal async Task ExecuteDelivery<T>(
            T message,
            MessageSlip messageSlip,
            DeliveryResult deliveryResult,
            string stepName = null)
            where T : class
        {
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