namespace Sapher.Configuration
{
    using System;

    public interface ISapherConfigurator
    {
        ISapherConfigurator AddStep(
            string name,
            Type inputHandlerType,
            Action<ISapherStepConfigurator> configure);

        ISapherConfigurator AddStep(
            string name,
            Type inputHandlerType);
    }
}