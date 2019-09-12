namespace Sapher.Handlers
{
    using System.Threading.Tasks;
    using Dtos;

    /// <summary>
    /// Defines a handler to an input message of a distributed transaction step
    /// </summary>
    /// <typeparam name="T">The input message to be handled</typeparam>
    public interface IHandlesInput<in T>
        : IHandlesInput
        where T : class
    {
        /// <summary>
        /// Executes the logic defined to handle the received message
        /// </summary>
        /// <param name="message">Input message</param>
        /// <param name="messageSlip">MessageSlip of the message, containing its identifiers</param>
        /// <returns>InputResult instance, containing the result of the execution</returns>
        Task<InputResult> Execute(T message, MessageSlip messageSlip);
    }

    /// <summary>
    /// Defines a handler to an input message of a distributed transaction step
    /// </summary>
    public interface IHandlesInput
    {

    }
}