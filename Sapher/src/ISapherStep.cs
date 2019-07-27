namespace Sapher
{
    using System.Threading.Tasks;
    using Persistence.Model;

    public interface ISapherStep
    {
        string StepName { get; set; }

        void Init();

        Task Deliver<T>(T message, MessageSlip messageSlip) where T : class;
    }
}