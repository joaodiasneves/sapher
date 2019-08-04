namespace Sapher
{
    using System;
    using System.Threading.Tasks;
    using Dtos;
    using Logger;

    public interface ISapherStep
    {
        string StepName { get; set; }

        void Init(IServiceProvider serviceProvider, ILogger logger);

        Task<StepResult> Deliver<T>(T message, Dtos.MessageSlip messageSlip) where T : class;
    }
}