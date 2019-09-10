namespace Sapher
{
    using System;

    internal interface IInternalSapher : ISapher
    {
        int TimeoutMs { get; }

        void Init(IServiceProvider serviceProvider);
    }
}