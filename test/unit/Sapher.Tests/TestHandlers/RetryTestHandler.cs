namespace Sapher.Tests.Handlers
{
    using System;
    using System.Threading.Tasks;
    using global::Sapher.Handlers;

    internal class ExceptionMessage
    {
        public Exception ExpectedException { get; set; }
    }

    internal class SapherExceptionMessage
    {
        public Exceptions.SapherException ExpectedException { get; set; }
    }

    internal class SapherConfigurationExceptionMessage
    {
        public Exceptions.SapherConfigurationException ExpectedException { get; set; }
    }

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