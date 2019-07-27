namespace Sapher.Persistence.Model
{
    public enum StepState
    {
        None = 0,
        Executed = 1,
        Successful = 2,
        Compensated = 3,
        FailedOnExecution = 4,
        FailedOnCompensation = 5,
        FailedOnSuccess = 6,
        MultipleStates = 7
        // TransactionCompleted = 5 TODO - Think about this later
    }
}