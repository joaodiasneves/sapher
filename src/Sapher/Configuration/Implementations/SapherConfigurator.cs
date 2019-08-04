namespace Sapher.Configuration.Implementations
{
    using System;
    using System.Collections.Generic;
    using Exceptions;
    using Handlers;
    using Logger;
    using Microsoft.Extensions.DependencyInjection;
    using Persistence;
    using Persistence.Repositories;

    public class SapherConfigurator : ISapherConfigurator
    {
        private readonly IServiceCollection serviceCollection;
        private readonly IList<ISapherStep> sapherSteps;
        private bool registeredLogger;
        private bool registeredPersistence;

        internal SapherConfigurator(IServiceCollection serviceCollection)
        {
            this.serviceCollection = serviceCollection;
            this.sapherSteps = new List<ISapherStep>();
        }

        internal ISapherConfiguration Configure()
        {
            if (!this.registeredLogger)
            {
                this.serviceCollection.AddTransient<ILogger, NullLogger>();
            }

            if (!this.registeredPersistence)
            {
                this.serviceCollection.AddTransient<ISapherDataRepository, InMemorySapherRepository>();
            }

            return new SapherConfiguration(this.sapherSteps);
        }

        public ISapherConfigurator AddStep<T>(
            string name,
            Action<ISapherStepConfigurator> configure = null)
        {
            var inputHandlerType = typeof(T);

            if (!HandlersFactory.TryToRegisterInputHandler(
                inputHandlerType,
                this.serviceCollection,
                out var inputMessageType,
                out var outputMessage))
            {
                throw new SapherException(outputMessage);
            }

            var stepConfigurator = new SapherStepConfigurator(
                name,
                inputMessageType,
                inputHandlerType,
                this.serviceCollection);

            configure?.Invoke(stepConfigurator);

            var stepConfiguration = stepConfigurator.Configure();

            this.sapherSteps.Add(new SapherStep(stepConfiguration));

            return this;
        }

        public ISapherConfigurator AddLogger<T>() where T : class, ILogger
        {
            this.serviceCollection.AddTransient<ILogger, T>();
            this.registeredLogger = true;
            return this;
        }

        public ISapherConfigurator AddPersistence<T>() where T : class, ISapherDataRepository
        {
            this.serviceCollection.AddTransient<ISapherDataRepository, T>();
            this.registeredPersistence = true;
            return this;
        }
    }
}