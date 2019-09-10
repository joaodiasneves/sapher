namespace Sapher.Dtos
{
    using System;

    /// <summary>
    /// DTO containing correlation information regarding the message, so that it can be tracked across the distributed transaction
    /// </summary>
    public class MessageSlip
    {
        /// <summary>
        /// The unique identifier of the message
        /// </summary>
        public string MessageId { get; }

        /// <summary>
        /// The ConversationId of a message equals the MessageId of the message that originated this one.
        /// If the message is the first message of the distributed transaction, ConversationId is null.
        /// </summary>
        public string ConversationId { get; }

        /// <summary>
        /// The CorrelationId of a message is a unique identifier of the distributed transaction.
        /// It is the same for all messages of a single distributed transaction.
        /// </summary>
        public string CorrelationId { get; }

        /// <summary>
        /// MessageSlip constructor.
        /// </summary>
        /// <param name="messageId">The message unique identifier. If no value is provided, a new Guid will be generated</param>
        /// <param name="conversationId">The conversationId.
        ///Sshould be the messageId of the message that originated this message.
        ///If the message is the first message of the distributed transaction, this should be left null</param>
        /// <param name="correlationId">The unique identifier of a distributed transaction. 
        /// Should be the same for all messages of a single distributed transaction. 
        /// If no value is provided, a new Guid will be generated.</param>
        public MessageSlip(string messageId = null, string conversationId = null, string correlationId = null)
        {
            this.MessageId = messageId ?? Guid.NewGuid().ToString();
            this.ConversationId = conversationId;
            this.CorrelationId = correlationId ?? Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Generates a new <c>MessageSlip</c> instance using the previous message <c>MessageSlip</c> defined in <paramref name="previousMessageSlip"/>
        /// </summary>
        /// <param name="previousMessageSlip">The MessageSlip of the previous Message</param>
        /// <returns>A new <c>MessageSlip</c> instance</returns>
        public static MessageSlip GenerateNewMessageSlip(MessageSlip previousMessageSlip)
            => new MessageSlip(
                previousMessageSlip.MessageId,
                previousMessageSlip.CorrelationId);

        /// <summary>
        /// Generates a new <c>MessageSlip</c> instance as the beginning of a distributed transaction
        /// </summary>
        /// <returns>A new <c>MessageSlip</c> instance</returns>
        public static MessageSlip GenerateNewMessageSlip()
            => new MessageSlip();
    }
}