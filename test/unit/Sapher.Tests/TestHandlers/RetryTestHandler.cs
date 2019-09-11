namespace Sapher.Tests.Handlers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using global::Sapher.Handlers;

    [ExcludeFromCodeCoverage]
    internal class ExceptionMessage
    {
        public Exception ExpectedException { get; set; }
    }

    [ExcludeFromCodeCoverage]
    internal class SapherExceptionMessage
    {
        public Exceptions.SapherException ExpectedException { get; set; }
    }

    [ExcludeFromCodeCoverage]
    internal class SapherConfigurationExceptionMessage
    {
        public Exceptions.SapherConfigurationException ExpectedException { get; set; }
    }

    [ExcludeFromCodeCoverage]
    internal class RetryExceptionTestHandler : IHandlesInput<ExceptionMessage>
    {
        // -1 to not consider the initial execution and provide only the number of retries executed.
        public static int ExecutionCount { get; set; } = -1;

        public Task<Dtos.InputResult> Execute(
            ExceptionMessage message,
            Dtos.MessageSlip messageSlip)
        {
            ExecutionCount++;
            throw message.ExpectedException;
        }
    }

    [ExcludeFromCodeCoverage]
    internal class RetrySapherExceptionTestHandler : IHandlesInput<SapherExceptionMessage>
    {
        // -1 to not consider the initial execution and provide only the number of retries executed.
        public static int ExecutionCount { get; set; } = -1;

        public Task<Dtos.InputResult> Execute(
            SapherExceptionMessage message,
            Dtos.MessageSlip messageSlip)
        {
            ExecutionCount++;
            throw message.ExpectedException;
        }
    }

    [ExcludeFromCodeCoverage]
    internal class RetrySapherConfigurationExceptionTestHandler : IHandlesInput<SapherConfigurationExceptionMessage>
    {
        // -1 to not consider the initial execution and provide only the number of retries executed.
        public static int ExecutionCount { get; set; } = -1;

        public Task<Dtos.InputResult> Execute(
            SapherConfigurationExceptionMessage message,
            Dtos.MessageSlip messageSlip)
        {
            ExecutionCount++;
            throw message.ExpectedException;
        }
    }
}