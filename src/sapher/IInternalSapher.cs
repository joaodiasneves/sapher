namespace Sapher
{
    using System;

    internal interface IInternalSapher : ISapher
    {
        int TimeoutInMinutes { get; }

        void Init(IServiceProvider serviceProvider);
    }
}