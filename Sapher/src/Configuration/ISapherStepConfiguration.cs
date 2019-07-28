﻿namespace Sapher.Configuration
{
    using System;
    using System.Collections.Generic;
    using Handlers;
    using Persistence;

    public interface ISapherStepConfiguration
    {
        string StepName { get; }

        Type InputMessageType { get; }

        IHandlesInput InputHandler { get; }

        ISapherDataRepository DataRepository { get; }

        IDictionary<Type, IHandlesResponse> ResponseHandlers { get; }
    }
}