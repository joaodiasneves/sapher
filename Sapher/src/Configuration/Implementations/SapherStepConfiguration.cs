namespace Sapher.Configuration.Implementations
{
    using System;
    using System.Collections.Generic;
    using Handlers;
    using Persistence;

    public class SapherStepConfiguration : ISapherStepConfiguration
    {
        public string StepName { get; }

        public Type InputMessageType { get; }

        public IHandlesStepInput InputHandler { get; }

        public ISapherDataRepository DataRepository { get; }

        public IDictionary<Type, IHandlesSuccess> SuccessHandlers { get; }

        public IDictionary<Type, IHandlesCompensation> CompensationHandlers { get; }

        public SapherStepConfiguration(
            string stepName,
            Type inputMessageType,
            IHandlesStepInput inputHandler,
            ISapherDataRepository dataRepository,
            IDictionary<Type, IHandlesSuccess> successHandlers,
            IDictionary<Type, IHandlesCompensation> compensationHandlers)
        {
            this.StepName = stepName;
            this.InputMessageType = inputMessageType;
            this.InputHandler = inputHandler;
            this.DataRepository = dataRepository;
            this.SuccessHandlers = successHandlers;
            this.CompensationHandlers = compensationHandlers;
        }
    }
}