namespace Sapher.Dtos
{
    using System;

    public class MessageSlip
    {
        public string MessageId { get; set; }

        public string ConversationId { get; set; }

        public string CorrelationId { get; set; }

        public MessageSlip(string messageId, string conversationId, string correlationId)
        {
            this.MessageId = messageId;
            this.ConversationId = conversationId;
            this.CorrelationId = correlationId;
        }

        public MessageSlip()
        {
        }

        public static MessageSlip GenerateNewMessageSlip(MessageSlip previousMessageSlip)
        {
            return new MessageSlip
            {
                MessageId = Guid.NewGuid().ToString(),
                ConversationId = previousMessageSlip.MessageId,
                CorrelationId = previousMessageSlip.CorrelationId ?? Guid.NewGuid().ToString()
            };
        }

        public static MessageSlip GenerateNewMessageSlip()
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