namespace Sapher.Configuration
{
    using System;
    using global::Sapher.Handlers;

    public interface ISapherConfigurator
    {
        ISapherConfigurator AddStep<T>(
            string name,
            Action<ISapherStepConfigurator> configure);

        ISapherConfigurator AddStep<T>(string name);
    }
}