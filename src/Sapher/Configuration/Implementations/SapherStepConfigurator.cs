namespace Sapher.Configuration.Implementations
{
    using System;
    using System.Collections.Generic;
    using Exceptions;
    using Handlers;
    using Microsoft.Extensions.DependencyInjection;
    using Utils;

    public class SapherStepConfigurator : ISapherStepConfigurator
    {
        private readonly string stepName;
        private readonly IServiceCollection serviceCollection;
        private readonly Type inputMessageType;
        private readonly Type inputHandlerType;
        private readonly IDictionary<Type, Type> responseHandlers;

        internal SapherStepConfigurator(
            string stepName,
            Type inputMessageType,
            Type inputHandlerType,
            IServiceCollection serviceCollection)
        {
            this.stepName = stepName;
            this.inputMessageType = inputMessageType;
            this.inputHandlerType = inputHandlerType;
            this.serviceCollection = serviceCollection;

            this.responseHandlers = new Dictionary<Type, Type>();
        }

        internal ISapherStepConfiguration Configure()
        {
            return new SapherStepConfiguration(
                this.stepName,
                this.inputMessageType,
                this.inputHandlerType,
                this.serviceCollection,
                this.responseHandlers);
        }

        public ISapherStepConfigurator AddResponseHandler<T>()
        {
            var responseHandlerType = typeof(T);

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
                throw new SapherConfigurationException(
                    outputMessage,
                    Pair.Of("StepName", this.stepName),
                    Pair.Of("ResponseHandler", responseHandlerType.Name));
            }

            foreach (var responseMessageType in responseMessageTypes)
            {
                if (responseMessageType == this.inputMessageType)
                {
                    throw new SapherException(
                        "Using the same Message as Input and Response in the same Step is not allowed.",
                        Pair.Of("StepName", this.stepName),
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