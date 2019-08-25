namespace Sapher.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using global::Sapher.Dtos;
    using TypeAdapters;

    public class InMemorySapherRepository : ISapherDataRepository
    {
        private readonly List<Model.SapherStepData> stepData = new List<Model.SapherStepData>();

        public Task<Dtos.SapherStepData> Load(string stepName, string inputMessageId)
            => Task.FromResult(this.stepData
                .Find(sd =>
                    string.Equals(
                        sd.Id,
                        Model.SapherStepData.GenerateId(stepName, inputMessageId),
                        StringComparison.InvariantCultureIgnoreCase))
                ?.ToDto());

        public Task<Dtos.SapherStepData> LoadFromConversationId(string stepName, string outputMessageId)
            => Task.FromResult(this.stepData
                .Find(sd =>
                    string.Equals(
                        sd.StepName,
                        stepName,
                        StringComparison.InvariantCultureIgnoreCase)
                    && sd.PublishedMessageIdsResponseState.Keys.Any(id =>
                    string.Equals(
                        id,
                        outputMessageId,
                        StringComparison.InvariantCultureIgnoreCase)))
                ?.ToDto());

        public Task UpdateInstancesState(Func<SapherStepData, bool> selector, StepState stepState)
        {
            var identifiedInstances = this.stepData
                    .Select(model => model.ToDto())
                    .Where(dto => selector(dto));

            foreach (var instance in identifiedInstances)
            {
                instance.State = stepState;
                this.Save(instance);
            }

            return Task.CompletedTask;
        }

        public Task<bool> Save(Dtos.SapherStepData data)
        {
            var model = data.ToDataModel();

            var result = this.LoadFromId(model.Id);
            if (result != null)
            {
                this.stepData.Remove(result);
            }

            this.stepData.Add(model);
            return Task.FromResult(true);
        }

        private Model.SapherStepData LoadFromId(string id)
            => this.stepData.Find(sd =>
                string.Equals(
                    sd.Id,
                    id,
                    StringComparison.InvariantCultureIgnoreCase));
    }
}