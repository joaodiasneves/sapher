namespace Sapher.Configuration
{
    using System;
    using Logger;
    using Persistence;

    public interface ISapherConfigurator
    {
        ISapherConfigurator AddStep<T>(string name, Action<ISapherStepConfigurator> configure = null);

        ISapherConfigurator AddLogger<T>() where T : class, ILogger;

        ISapherConfigurator AddPersistence<T>() where T : class, ISapherDataRepository;

        ISapherConfigurator AddRetryPolicy(int maxRetryAttempts = 3, int retryIntervalMs = 3000);

        ISapherConfigurator AddTimeoutPolicy(int timeoutInMinutes = 30);
    }
}