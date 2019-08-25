namespace Sapher.Configuration
{
    using System.Collections.Generic;

    public interface ISapherConfiguration
    {
        IList<ISapherStep> SapherSteps { get; }

        int MaxRetryAttempts { get; }

        int RetryIntervalMs { get; }

        int TimeoutInMinutes { get; }
    }
}