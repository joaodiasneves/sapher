namespace Sapher.Persistence.Model
{
    internal enum StepState
    {
        None = 0,
        ExecutedInput = 1,
        Successful = 2,
        Compensated = 3,
        FailedOnExecution = 4,
        FailedOnResponses = 5,
        Timeout = 6
    }
}