namespace Sapher.Configuration.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
                this.serviceCollection,
                this.responseHandlers);
        }

        public ISapherStepConfigurator AddResponseHandler(Type responseHandlerType)
        {
            if (this.responseHandlers.Values.Contains(responseHandlerType))
            {
                return this;
            }

            if (!HandlersFactory.TryToRegisterResponseHandler(
                responseHandlerType,
                this.serviceCollection,
                out var responseMessageTypes,
                out var outputMessage))
            {
                throw new SapherException(outputMessage); // TODO IMprove exceptions
            }
           
            foreach (var responseMessageType in responseMessageTypes)
            {
                if(responseMessageType == inputMessageType)
                {
                    throw new SapherException(
                        "Using the same Message as Input and Response in the same Step is not allowed.",
                        Pair.Of("StepName", this.name),
                        Pair.Of("MessageType", responseMessageType.Name),
                        Pair.Of("InputHandler", this.inputHandlerType.Name),
                        Pair.Of("ResponseHandler", responseHandlerType.Name));
                }

                this.responseHandlers.Add(responseMessageType, responseHandlerType);
            }
            
            return this;
        }
    }
}