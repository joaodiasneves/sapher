namespace Sapher.Dtos
{
    using System.Collections.Generic;

    /// <summary>
    /// DTO to provide information regarding the result of an input handler execution
    /// </summary>
    public class InputResult : HandlerResult
    {
        /// <summary>
        /// The result State of the input handler execution
        /// </summary>
        public InputResultState State { get; set; }

        /// <summary>
        /// Contains the list of Message Ids of the messages sent (or published) by the input handler execution.
        /// This is used for mapping the responses received and tracking the state of each message.
        /// </summary>
        public IEnumerable<string> SentMessageIds { get; set; } = new List<string>();
    }
}