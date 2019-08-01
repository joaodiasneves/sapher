namespace Sapher
{
    using System.Threading.Tasks;
    using Dtos;

    public interface ISapherStep
    {
        string StepName { get; set; }

        void Init();

        Task<StepResult> Deliver<T>(T message, Dtos.MessageSlip messageSlip) where T : class;
    }
}