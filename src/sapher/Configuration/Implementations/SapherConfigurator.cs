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
    using Utils;

    /// <summary>
    /// Provides configuration functionalities for Sapher features
    /// </summary>
    public class SapherConfigurator : ISapherConfigurator
    {
        private readonly IServiceCollection serviceCollection;
        private readonly IList<ISapherStep> sapherSteps;
        private Type loggerType = typeof(NullLogger);
        private ILogger loggerInstance;
        private Type persistenceType = typeof(InMemorySapherRepository);
        private ISapherDataRepository persistenceInstance;
        private int maxRetryAttempts;
        private int retryIntervalMs;
        private int timeoutMs;

        internal SapherConfigurator(IServiceCollection serviceCollection)
        {
            this.serviceCollection = serviceCollection;
            this.sapherSteps = new List<ISapherStep>();
        }

        internal ISapherConfiguration Configure()
        {
            if (loggerInstance != null)
            {
                this.serviceCollection.AddSingleton(loggerInstance);
            }
            else
            {
                this.serviceCollection.AddSingleton(typeof(ILogger), loggerType);
            }

            if (persistenceInstance != null)
            {
                this.serviceCollection.AddSingleton(persistenceInstance);
            }
            else
            {
                this.serviceCollection.AddSingleton(typeof(ISapherDataRepository), persistenceType);
            }

            return new SapherConfiguration(this.sapherSteps, this.maxRetryAttempts, this.retryIntervalMs, this.timeoutMs);
        }

        /// <summary>
        /// Adds a SapherStep to Sapher configuration
        /// </summary>
        /// <typeparam name="T">InputHandler type</typeparam>
        /// <param name="name">Step name</param>
        /// <param name="configure">Configuration action for the Step creation</param>
        /// <returns>Updated ISapherConfigurator for fluent configuration</returns>
        public ISapherConfigurator AddStep<T>(
            string name,
            Action<ISapherStepConfigurator> configure = null)
            where T : class, IHandlesInput

        {
            var inputHandlerType = typeof(T);

            if (!HandlersFactory.TryToRegisterInputHandler(
                inputHandlerType,
                this.serviceCollection,
                out var inputMessageType,
                out var outputMessage))
            {
                throw new SapherConfigurationException(
                    outputMessage,
                    Pair.Of("StepName", name),
                    Pair.Of("InputHandler", inputHandlerType.Name));
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

        /// <summary>
        /// Defines an implementation of ILogger to be used by Sapher for logging.
        /// If not defined, Sapher will not log anything.
        /// This is used as singleton.
        /// </summary>
        /// <typeparam name="T">Implementation of ILogger</typeparam>
        /// <param name="instance">Instance of logger to be used</param>
        /// <returns>Updated ISapherConfigurator for fluent configuration</returns>
        public ISapherConfigurator AddLogger<T>(T instance = null) where T : class, ILogger
        {
            this.loggerInstance = instance;
            this.loggerType = typeof(T);
            return this;
        }

        /// <summary>
        /// Defines an implementation of ISapherDataRepository to be used by Sapher for persistence.
        /// If not defined, Sapher will use In Memory persistence.
        /// This is used as singleton.
        /// </summary>
        /// <typeparam name="T">Implementation of ISapherDataRepository</typeparam>
        /// <param name="instance">Instance of Repository to be used</param>
        /// <returns>Updated ISapherConfigurator for fluent configuration</returns>
        public ISapherConfigurator AddPersistence<T>(T instance = null) where T : class, ISapherDataRepository
        {
            this.persistenceInstance = instance;
            this.persistenceType = typeof(T);
            return this;
        }

        /// <summary>
        /// Defines the policy for retry mechanisms.
        /// If not defined, Sapher will not execute retries.
        /// </summary>
        /// <param name="maxRetryAttempts">Maximum number of Retries to be executed. Default is 3</param>
        /// <param name="retryIntervalMs">Internal in milliseconds that Sapher will wait between retries. Default is 3000</param>
        /// <returns>Updated ISapherConfigurator for fluent configuration</returns>
        public ISapherConfigurator AddRetryPolicy(int maxRetryAttempts = 3, int retryIntervalMs = 3000)
        {
            this.maxRetryAttempts = maxRetryAttempts;
            this.retryIntervalMs = retryIntervalMs;

            return this;
        }

        /// <summary>
        /// Defines the policy for Timeout mechanisms.
        /// If not defined, executions that are waiting responses will wait forever.
        /// </summary>
        /// <param name="timeoutMs">time in milliseconds to wait before marking execution as timed out in milliseconds</param>
        /// <returns>Updated ISapherConfigurator for fluent configuration</returns>
        public ISapherConfigurator AddTimeoutPolicy(int timeoutMs = 30)
        {
            this.timeoutMs = timeoutMs;
            this.serviceCollection.AddHostedService<TimeoutHostedService>();

            return this;
        }
    }
}