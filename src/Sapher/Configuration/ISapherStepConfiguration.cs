namespace Sapher.Configuration
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.DependencyInjection;
    using Persistence;

    public interface ISapherStepConfiguration
    {
        string StepName { get; }

        Type InputMessageType { get; }

        Type InputHandlerType { get; }

        ISapherDataRepository DataRepository { get; }

        IDictionary<Type, Type> ResponseHandlers { get; }

        IServiceCollection ServiceCollection { get; }
    }
}