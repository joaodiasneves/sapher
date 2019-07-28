namespace Sapher.Configuration.Implementations
{
    using System;
    using System.Collections.Generic;
    using Exceptions;
    using Handlers;
    using Microsoft.Extensions.DependencyInjection;
    using Persistence;
    using Persistence.Repositories;

    public class SapherConfigurator : ISapherConfigurator
    {
        private readonly IServiceCollection serviceCollection;
        private readonly IList<ISapherStep> sapherSteps;
        private readonly ISapherDataRepository dataRepository;

        internal SapherConfigurator(IServiceCollection serviceCollection)
        {
            this.serviceCollection = serviceCollection;
            this.sapherSteps = new List<ISapherStep>();
            this.dataRepository = new InMemorySapherRepository(); // TODO - Add Other Persistence
        }

        internal ISapherConfiguration Configure()
        {
            return new SapherConfiguration(sapherSteps);
        }

        public ISapherConfigurator AddStep(
            string name,
            Type inputHandlerType,
            Action<ISapherStepConfigurator> configure)
        {
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
                this.dataRepository,
                this.serviceCollection);

            configure(stepConfigurator);
            var stepConfiguration = stepConfigurator.Configure();

            this.sapherSteps.Add(new SapherStep(stepConfiguration));

            return this;
        }
    }
}