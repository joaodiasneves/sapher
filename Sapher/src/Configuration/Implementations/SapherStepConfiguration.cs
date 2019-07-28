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

        public IHandlesInput InputHandler { get; }

        public ISapherDataRepository DataRepository { get; }

        public IDictionary<Type, IHandlesResponse> ResponseHandlers { get; }

        public SapherStepConfiguration(
            string stepName,
            Type inputMessageType,
            IHandlesInput inputHandler,
            ISapherDataRepository dataRepository,
            IDictionary<Type, IHandlesResponse> responseHandlers)
        {
            this.StepName = stepName;
            this.InputMessageType = inputMessageType;
            this.InputHandler = inputHandler;
            this.DataRepository = dataRepository;
            this.ResponseHandlers = responseHandlers;
        }
    }
}