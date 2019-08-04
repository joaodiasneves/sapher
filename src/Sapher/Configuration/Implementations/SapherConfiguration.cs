namespace Sapher.Configuration.Implementations
{
    using System.Collections.Generic;

    public class SapherConfiguration : ISapherConfiguration
    {
        public IList<ISapherStep> SapherSteps { get; }

        public int MaxRetryAttempts { get; }

        public int RetryIntervalMs { get; }

        public SapherConfiguration(
            IList<ISapherStep> sapherSteps,
            int maxRetryAttempts,
            int retryIntervalMs)
        {
            this.SapherSteps = sapherSteps;
            this.MaxRetryAttempts = maxRetryAttempts;
            this.RetryIntervalMs = retryIntervalMs;
        }
    }
}