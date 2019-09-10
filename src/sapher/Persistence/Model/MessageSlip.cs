namespace Sapher.Persistence.Model
{
    using System;

    internal class MessageSlip
    {
        internal string MessageId { get; set; }

        internal string ConversationId { get; set; }

        internal string CorrelationId { get; set; }

        internal MessageSlip(string messageId, string conversationId, string correlationId)
        {
            this.MessageId = messageId;
            this.ConversationId = conversationId;
            this.CorrelationId = correlationId;
        }

        internal MessageSlip()
        {
        }

        internal static MessageSlip GenerateNewMessageSlip(MessageSlip previousMessageSlip)
        {
            return new MessageSlip
            {
                MessageId = Guid.NewGuid().ToString(),
                ConversationId = previousMessageSlip.MessageId,
                CorrelationId = previousMessageSlip.CorrelationId ?? Guid.NewGuid().ToString()
            };
        }

        internal static MessageSlip GenerateNewMessageSlip()
        {
            return new MessageSlip
            {
                MessageId = Guid.NewGuid().ToString(),
                ConversationId = null,
                CorrelationId = Guid.NewGuid().ToString()
            };
        }
    }
}