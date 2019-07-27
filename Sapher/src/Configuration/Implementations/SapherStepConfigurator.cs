namespace Sapher.Configuration.Implementations
{
    using System;
    using System.Collections.Generic;
    using Exceptions;
    using Persistence;
    using Handlers;
    using Microsoft.Extensions.DependencyInjection;

    public class SapherStepConfigurator : ISapherStepConfigurator
    {
        private readonly string name;
        private readonly Type inputMessageType;
        private readonly IHandlesStepInput inputHandler;
        private readonly ISapherDataRepository dataRepository;
        private readonly IServiceCollection serviceCollection;
        private readonly IDictionary<Type, IHandlesSuccess> successHandlers;
        private readonly IDictionary<Type, IHandlesCompensation> compensationHandlers;

        internal SapherStepConfigurator(
            string name,
            Type inputMessageType,
            IHandlesStepInput inputHandler,
            ISapherDataRepository dataRepository,
            IServiceCollection serviceCollection)
        {
            this.name = name;
            this.inputMessageType = inputMessageType;
            this.inputHandler = inputHandler;
            this.dataRepository = dataRepository;
            this.serviceCollection = serviceCollection;

            this.successHandlers = new Dictionary<Type, IHandlesSuccess>();
            this.compensationHandlers = new Dictionary<Type, IHandlesCompensation>();
        }

        internal ISapherStepConfiguration Configure()
        {
            return new SapherStepConfiguration(
                this.name,
                this.inputMessageType,
                this.inputHandler,
                this.dataRepository,
                this.successHandlers,
                this.compensationHandlers);
        }

        public ISapherStepConfigurator AddSuccessHandler(Type successHandlerType)
        {
            if (!HandlersFactory.TryToGenerateHandlerInfo<IHandlesSuccess>(
                successHandlerType,
                this.serviceCollection,
                out var successHandlerInstance,
                out var successMessageType,
                out var outputMessage))
            {
                throw new SapherException(outputMessage);
            }
            // TODO - VAlidate that the GenericTypeDefinition is not used in input or compensation handlers

            this.successHandlers.Add(successMessageType, successHandlerInstance);
            return this;
        }

        public ISapherStepConfigurator AddCompensationHandler(Type compensationHandlerType)
        {
            if (!HandlersFactory.TryToGenerateHandlerInfo<IHandlesCompensation>(
                compensationHandlerType,
                this.serviceCollection,
                out var compensationHandlerInstance,
                out var compensationMessageType,
                out var outputMessage))
            {
                throw new SapherException(outputMessage);
            }

            // TODO - VAlidate that the GenericTypeDefinition is not used in input or compensation handlers
            this.compensationHandlers.Add(compensationMessageType, compensationHandlerInstance);
            return this;
        }
    }
}