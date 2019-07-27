namespace Sapher
{
    using Persistence.Model;

    public interface ISapher
    {
        void Init();

        void DeliverMessage<T>(T message, MessageSlip messageSlip, string stepName = null) where T : class;
    }
}