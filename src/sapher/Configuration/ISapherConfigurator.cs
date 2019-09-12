namespace Sapher.Configuration
{
    using System;
    using Handlers;
    using Logger;
    using Persistence;

    /// <summary>
    /// Provides configuration functionalities for Sapher features
    /// </summary>
    public interface ISapherConfigurator
    {
        /// <summary>
        /// Adds a SapherStep to Sapher configuration
        /// </summary>
        /// <typeparam name="T">InputHandler type</typeparam>
        /// <param name="name">Step name</param>
        /// <param name="configure">Configuration action for the Step creation</param>
        /// <returns>Updated ISapherConfigurator for fluent configuration</returns>
        ISapherConfigurator AddStep<T>(string name, Action<ISapherStepConfigurator> configure = null)
            where T : class, IHandlesInput;

        /// <summary>
        /// Defines an implementation of ILogger to be used by Sapher for logging.
        /// If not defined, Sapher will not log anything.
        /// This is used as singleton.
        /// </summary>
        /// <typeparam name="T">Implementation of ILogger</typeparam>
        /// <param name="instance">Instance of logger to be used</param>
        /// <returns>Updated ISapherConfigurator for fluent configuration</returns>
        ISapherConfigurator AddLogger<T>(T instance = null) where T : class, ILogger;

        /// <summary>
        /// Defines an implementation of ISapherDataRepository to be used by Sapher for persistence.
        /// If not defined, Sapher will use In Memory persistence.
        /// This is used as singleton.
        /// </summary>
        /// <typeparam name="T">Implementation of ISapherDataRepository</typeparam>
        /// <param name="instance">Instance of Repository to be used</param>
        /// <returns>Updated ISapherConfigurator for fluent configuration</returns>
        ISapherConfigurator AddPersistence<T>(T instance = null) where T : class, ISapherDataRepository;

        /// <summary>
        /// Defines the policy for retry mechanisms.
        /// If not defined, Sapher will not execute retries.
        /// </summary>
        /// <param name="maxRetryAttempts">Maximum number of Retries to be executed. Default is 3</param>
        /// <param name="retryIntervalMs">Internal in milliseconds that Sapher will wait between retries. Default is 3000</param>
        /// <returns>Updated ISapherConfigurator for fluent configuration</returns>
        ISapherConfigurator AddRetryPolicy(int maxRetryAttempts = 3, int retryIntervalMs = 3000);

        /// <summary>
        /// Defines the policy for Timeout mechanisms.
        /// If not defined, executions that are waiting responses will wait forever.
        /// </summary>
        /// <param name="timeoutMs">time in milliseconds to wait before marking execution as timed out in milliseconds</param>
        /// <returns>Updated ISapherConfigurator for fluent configuration</returns>
        ISapherConfigurator AddTimeoutPolicy(int timeoutMs = 30);
    }
}