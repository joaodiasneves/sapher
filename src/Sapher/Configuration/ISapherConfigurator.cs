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
    }
}