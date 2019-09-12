namespace Sapher.Configuration.Implementations
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.DependencyInjection;

    internal class SapherStepConfiguration : ISapherStepConfiguration
    {
        public string StepName { get; }

        public Type InputMessageType { get; }

        public Type InputHandlerType { get; }

        public IDictionary<Type, Type> ResponseHandlers { get; }

        public IServiceCollection ServiceCollection { get; }

        public SapherStepConfiguration(
            string stepName,
            Type inputMessageType,
            Type inputHandlerType,
            IServiceCollection serviceCollection,
            IDictionary<Type, Type> responseHandlers)
        {
            this.StepName = stepName;
            this.InputMessageType = inputMessageType;
            this.InputHandlerType = inputHandlerType;
            this.ServiceCollection = serviceCollection;
            this.ResponseHandlers = responseHandlers;
        }
    }
}