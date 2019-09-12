namespace Sapher
{
    using System;
    using System.Threading.Tasks;
    using Sapher.Dtos;
    using Sapher.Logger;

    internal interface ISapherStep
    {
        string StepName { get; set; }

        void Init(IServiceProvider serviceProvider, ILogger logger);

        Task<StepResult> Deliver<T>(T message, MessageSlip messageSlip) where T : class;
    }
}