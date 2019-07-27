namespace Sapher.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Persistence.Model;

    public class InMemorySapherRepository : ISapherDataRepository
    {
        private readonly List<SapherStepData> stepData = new List<SapherStepData>();

        public SapherStepData Load(string stepName, string inputMessageId)
            => this.stepData.Find(sd =>
                string.Equals(
                    sd.Id,
                    SapherStepData.GenerateId(stepName, inputMessageId),
                    StringComparison.InvariantCultureIgnoreCase));

        public SapherStepData Load(string id)
            => this.stepData.Find(sd =>
                string.Equals(
                    sd.Id,
                    id,
                    StringComparison.InvariantCultureIgnoreCase));

        public SapherStepData LoadFromConversationId(string stepName, string outputMessageId)
            => this.stepData.Find(sd =>
                string.Equals(
                    sd.StepName,
                    stepName,
                    StringComparison.InvariantCultureIgnoreCase)
                && sd.OutputMessageIdsState.Keys.Any(id =>
                string.Equals(
                    id,
                    outputMessageId,
                    StringComparison.InvariantCultureIgnoreCase)));

        public void Save(SapherStepData data)
        {
            var result = this.Load(data.Id);
            if(result != null)
            {
                this.stepData.Remove(result);
            }

            this.stepData.Add(data);
        }
    }
}