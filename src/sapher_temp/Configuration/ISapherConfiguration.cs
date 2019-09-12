namespace Sapher.Configuration
{
    using System.Collections.Generic;

    internal interface ISapherConfiguration
    {
        IList<ISapherStep> SapherSteps { get; }

        int MaxRetryAttempts { get; }

        int RetryIntervalMs { get; }

        int TimeoutInMinutes { get; }
    }
}