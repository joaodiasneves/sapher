namespace Sapher.Configuration.Implementations
{
    using System.Collections.Generic;

    public class SapherConfiguration : ISapherConfiguration
    {
        public IList<ISapherStep> SapherSteps { get; }

        public SapherConfiguration(IList<ISapherStep> sapherSteps)
        {
            this.SapherSteps = sapherSteps;
        }
    }
}