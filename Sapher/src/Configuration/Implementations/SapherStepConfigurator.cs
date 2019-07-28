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
        private readonly ISapherDataRepository dataRepository;
        private readonly IServiceCollection serviceCollection;
        private readonly Type inputMessageType;
        private readonly Type inputHandlerType;
        private readonly IDictionary<Type, Type> responseHandlers;

        internal SapherStepConfigurator(
            string name,
            Type inputMessageType,
            Type inputHandlerType,
            ISapherDataRepository dataRepository,
            IServiceCollection serviceCollection)
        {
            this.name = name;
            this.inputMessageType = inputMessageType;
            this.inputHandlerType = inputHandlerType;
            this.dataRepository = dataRepository;
            this.serviceCollection = serviceCollection;

            this.responseHandlers = new Dictionary<Type, Type>();
        }

        internal ISapherStepConfiguration Configure()
        {
            return new SapherStepConfiguration(
                this.name,
                this.inputMessageType,
                this.inputHandlerType,
                this.dataRepository,
                this.responseHandlers);
        }

        public ISapherStepConfigurator AddResponseHandler(Type responseHandlerType)
        {
            if (!HandlersFactory.TryToRegisterResponseHandler(
                responseHandlerType,
                this.serviceCollection,
                out var responseMessageType,
                out var outputMessage))
            {
                throw new SapherException(outputMessage); // TODO IMprove exceptions
            }

            this.responseHandlers.Add(responseMessageType, responseHandlerType);
            return this;
        }
    }
}