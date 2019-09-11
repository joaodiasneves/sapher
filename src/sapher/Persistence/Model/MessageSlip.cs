namespace Sapher.Persistence.Model
{
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
    }
}