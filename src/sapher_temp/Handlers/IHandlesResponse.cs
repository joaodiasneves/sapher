namespace Sapher.Handlers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Dtos;

    /// <summary>
    /// Defines a handler to a response message of a distributed transaction step
    /// </summary>
    /// <typeparam name="T">The response message to be handled</typeparam>
    public interface IHandlesResponse<in T>
        : IHandlesResponse
        where T : class
    {
        /// <summary>
        /// Executes the logic defined to handle the received message
        /// </summary>
        /// <param name="message">Received response message</param>
        /// <param name="messageSlip">MessageSlip of the message, containing its identifiers</param>
        /// <param name="previouslyPersistedData">Data provided to be persisted in the distributed transaction step input execution 
        /// - provided by the InputResult returned by the InputHandler's execute method</param>
        /// <returns>ResponseResult instance, containing the result of the execution</returns>
        Task<ResponseResult> Execute(
            T message,
            MessageSlip messageSlip,
            IDictionary<string, string> previouslyPersistedData);
    }

    /// <summary>
    /// Defines a handler to a response message of a distributed transaction step
    /// </summary>
    public interface IHandlesResponse
    {

    }
}