namespace Sapher.Configuration
{
    using System;

    public interface ISapherStepConfigurator
    {
        ISapherStepConfigurator AddSuccessHandler(Type successHandlerType);

        ISapherStepConfigurator AddCompensationHandler(Type compensationHandlerType);
    }
}