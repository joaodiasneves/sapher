namespace Sapher.Configuration
{
    using System;
    using System.Collections.Generic;
    using Handlers;
    using Persistence;

    public interface ISapherStepConfiguration
    {
        string StepName { get; }

        Type InputMessageType { get; }

        IHandlesStepInput InputHandler { get; }

        ISapherDataRepository DataRepository { get; }

        IDictionary<Type, IHandlesSuccess> SuccessHandlers { get; }

        IDictionary<Type, IHandlesCompensation> CompensationHandlers { get; }
    }
}