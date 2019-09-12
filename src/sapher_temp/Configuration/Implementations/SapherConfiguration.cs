namespace Sapher.Configuration.Implementations
{
    using System.Collections.Generic;

    internal class SapherConfiguration : ISapherConfiguration
    {
        public IList<ISapherStep> SapherSteps { get; }

        public int MaxRetryAttempts { get; }

        public int RetryIntervalMs { get; }

        public int TimeoutInMinutes { get; }

        public SapherConfiguration(
            IList<ISapherStep> sapherSteps,
            int maxRetryAttempts,
            int retryIntervalMs,
            int timeoutInMinutes)
        {
            this.SapherSteps = sapherSteps;
            this.MaxRetryAttempts = maxRetryAttempts;
            this.RetryIntervalMs = retryIntervalMs;
            this.TimeoutInMinutes = timeoutInMinutes;
        }
    }
}