namespace Sapher
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Dtos;

    /// <summary>
    /// Base interface for Sapher execution
    /// </summary>
    public interface ISapher
    {
        /// <summary>
        /// Delivers a message across the configured SapherSteps.
        /// Scans all the Input and Response handlers and delivers the message to the respective message handlers.
        /// </summary>
        /// <typeparam name="T">Type of the Message to be delivered</typeparam>
        /// <param name="message">Message to be delivered</param>
        /// <param name="messageSlip">MessageSlip fo the message to be delivered and its correspondent identifiers</param>
        /// <param name="stepName">To deliver the message to a specific step, the wanted StepName should be provided</param>
        /// <returns>The result of the Delivery. Containing all the executed steps and their respective results.</returns>
        Task<DeliveryResult> DeliverMessage<T>(T message, MessageSlip messageSlip, string stepName = null) where T : class;

        /// <summary>
        /// Provides current information regarding an instance specified with <paramref name="stepName"/> and <paramref name="inputMessageId"/>
        /// </summary>
        /// <param name="stepName">The name of the step to read</param>
        /// <param name="inputMessageId">The name of the input message id in order to identify the correct step instance</param>
        /// <returns>Sapher information regarding the mentioned step</returns>
        Task<SapherStepData> GetStepInstance(string stepName, string inputMessageId);

        /// <summary>
        /// Provides information regarding all instances of the step specified by <paramref name="stepName"/>. Paginated with <paramref name="page"/> and <paramref name="pageSize"/>
        /// </summary>
        /// <param name="stepName">The name of the step to read</param>
        /// <param name="page">The number of the page to retrieve.</param>
        /// <param name="pageSize">The number of items to retrieve per page.</param>
        /// <returns>Sapher information regarding the mentioned step</returns>
        Task<IEnumerable<SapherStepData>> GetStepInstances(string stepName, int page = 0, int pageSize = 100);
    }
}