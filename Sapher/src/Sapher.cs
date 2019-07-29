namespace Sapher
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Configuration;
    using Dtos;
    using Exceptions;

    public class Sapher : ISapher
    {
        private readonly ISapherConfiguration configuration;
        private IEnumerable<ISapherStep> steps = new List<ISapherStep>();

        internal Sapher(ISapherConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void Init()
        {
            SetupSteps();
        }

        internal void SetupSteps()
        {
            if (this.configuration.SapherSteps?.Any() == true)
            {
                this.steps = this.configuration.SapherSteps;

                foreach (var step in this.steps)
                {
                    step.Init();
                }
            }
            else
            {
                throw new SapherException("Trying to Use Sapher without defining any step"); // TODO - Improve exceptions
            }
        }

        public async Task<DeliveryResult> DeliverMessage<T>(
            T message,
            MessageSlip messageSlip,
            string stepName = null)
            where T : class
        {
            var affectedSteps = this.steps;

            if (!string.IsNullOrWhiteSpace(stepName))
            {
                affectedSteps = affectedSteps
                    .Where(s =>
                        string.Equals(
                            s.StepName,
                            stepName,
                            System.StringComparison.InvariantCultureIgnoreCase));
            }

            var tasks = new List<Task<StepResult>>();

            foreach (var step in affectedSteps)
            {
                tasks.Add(step.Deliver(message, messageSlip));
            }

            var stepResults = await Task.WhenAll(tasks).ConfigureAwait(false);
            return new DeliveryResult
            {
                StepsExecuted = stepResults.ToList()
            };
        }
    }
}