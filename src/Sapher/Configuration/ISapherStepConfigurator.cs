﻿namespace Sapher.Configuration
{
    using System;

    public interface ISapherStepConfigurator
    {
        ISapherStepConfigurator AddResponseHandler<T>();
    }
}