namespace Sapher
{
    internal interface IInternalSapher : ISapher
    {
        int TimeoutInMinutes { get; }
    }
}