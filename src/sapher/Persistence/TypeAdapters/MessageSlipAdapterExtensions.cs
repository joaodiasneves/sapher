namespace Sapher.Persistence.TypeAdapters
{
    using Model = Persistence.Model;

    internal static class MessageSlipAdapterExtensions
    {
        internal static Model.MessageSlip ToDataModel(this Dtos.MessageSlip messageSlip)
            => new Model.MessageSlip
            {
                MessageId = messageSlip.MessageId,
                ConversationId = messageSlip.ConversationId,
                CorrelationId = messageSlip.CorrelationId
            };

        internal static Dtos.MessageSlip ToDto(this Model.MessageSlip messageSlip)
            => new Dtos.MessageSlip
            {
                MessageId = messageSlip.MessageId,
                ConversationId = messageSlip.ConversationId,
                CorrelationId = messageSlip.CorrelationId
            };
    }
}