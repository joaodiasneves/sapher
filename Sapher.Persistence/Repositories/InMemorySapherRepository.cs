namespace Sapher.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Persistence.Model;

    public class InMemorySapherRepository : ISapherDataRepository
    {
        private readonly List<SapherStepData> stepData = new List<SapherStepData>();

        public Task<SapherStepData> Load(string stepName, string inputMessageId)
            => Task.FromResult(this.stepData.Find(sd =>
                string.Equals(
                    sd.Id,
                    SapherStepData.GenerateId(stepName, inputMessageId),
                    StringComparison.InvariantCultureIgnoreCase)));

        public Task<SapherStepData> Load(string id)
            => Task.FromResult(this.stepData.Find(sd =>
                string.Equals(
                    sd.Id,
                    id,
                    StringComparison.InvariantCultureIgnoreCase)));

        public Task<SapherStepData> LoadFromConversationId(string stepName, string outputMessageId)
            => Task.FromResult(this.stepData.Find(sd =>
                string.Equals(
                    sd.StepName,
                    stepName,
                    StringComparison.InvariantCultureIgnoreCase)
                && sd.PublishedMessageIdsResponseState.Keys.Any(id =>
                string.Equals(
                    id,
                    outputMessageId,
                    StringComparison.InvariantCultureIgnoreCase))));

        public async Task<bool> Save(SapherStepData data)
        {
            var result = await this.Load(data.Id).ConfigureAwait(false);
            if (result != null)
            {
                this.stepData.Remove(result);
            }

            this.stepData.Add(data);
            return true;
        }
    }
}