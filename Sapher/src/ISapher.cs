namespace Sapher
{
    using System.Threading.Tasks;
    using Dtos;

    public interface ISapher
    {
        void Init();

        Task<DeliveryResult> DeliverMessage<T>(T message, MessageSlip messageSlip, string stepName = null) where T : class;
    }
}