namespace Sapher.Configuration
{
    using System.Collections.Generic;

    public interface ISapherConfiguration
    {
        IList<ISapherStep> SapherSteps { get; }
    }
}