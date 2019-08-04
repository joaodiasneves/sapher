namespace Sapher.Persistence.TypeAdapters
{
    using Model = Persistence.Model;

    public static class MessageSlipAdapterExtensions
    {
        public static Model.MessageSlip ToDataModel(this Dtos.MessageSlip messageSlip)
            => new Model.MessageSlip
            {
                MessageId = messageSlip.MessageId,
                ConversationId = messageSlip.ConversationId,
                CorrelationId = messageSlip.CorrelationId
            };

        public static Dtos.MessageSlip ToDto(this Model.MessageSlip messageSlip)
            => new Dtos.MessageSlip
            {
                MessageId = messageSlip.MessageId,
                ConversationId = messageSlip.ConversationId,
                CorrelationId = messageSlip.CorrelationId
            };
    }
}