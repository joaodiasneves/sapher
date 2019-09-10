namespace Sapher
{
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
    }
}