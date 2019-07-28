namespace Sapher
{
    using System.Threading.Tasks;

    public interface ISapherStep
    {
        string StepName { get; set; }

        void Init();

        Task Deliver<T>(T message, Dtos.MessageSlip messageSlip) where T : class;
    }
}