namespace Sapher.Configuration.Implementations
{
    using System;
    using System.Collections.Generic;
    using Exceptions;
    using Handlers;
    using Microsoft.Extensions.DependencyInjection;
    using Persistence;

    public class SapherStepConfigurator : ISapherStepConfigurator
    {
        private readonly string name;
        private readonly Type inputMessageType;
        private readonly IHandlesInput inputHandler;
        private readonly ISapherDataRepository dataRepository;
        private readonly IServiceCollection serviceCollection;
        private readonly IDictionary<Type, IHandlesResponse> responseHandlers;

        internal SapherStepConfigurator(
            string name,
            Type inputMessageType,
            IHandlesInput inputHandler,
            ISapherDataRepository dataRepository,
            IServiceCollection serviceCollection)
        {
            this.name = name;
            this.inputMessageType = inputMessageType;
            this.inputHandler = inputHandler;
            this.dataRepository = dataRepository;
            this.serviceCollection = serviceCollection;

            this.responseHandlers = new Dictionary<Type, IHandlesResponse>();
        }

        internal ISapherStepConfiguration Configure()
        {
            return new SapherStepConfiguration(
                this.name,
                this.inputMessageType,
                this.inputHandler,
                this.dataRepository,
                this.responseHandlers);
        }

        public ISapherStepConfigurator AddResponseHandler(Type responseHandlerType)
        {
            if (!HandlersFactory.TryToGenerateHandlerInfo<IHandlesResponse>(
                responseHandlerType,
                this.serviceCollection,
                out var responseHandlerInstance,
                out var responseMessageType,
                out var outputMessage))
            {
                throw new SapherException(outputMessage); // TODO IMprove exceptions
            }

            this.responseHandlers.Add(responseMessageType, responseHandlerInstance);
            return this;
        }
    }
}