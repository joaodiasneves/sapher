namespace Sapher.Persistence.Model
{
    public enum OutputState
    {
        None = 0,
        Successful = 1,
        Compensated = 2,
        FailedOnSuccess = 3,
        FailedOnCompensation = 4
    }
}