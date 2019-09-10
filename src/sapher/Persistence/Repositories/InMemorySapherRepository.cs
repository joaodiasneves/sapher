namespace Sapher.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Dtos;
    using TypeAdapters;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class InMemorySapherRepository : ISapherDataRepository
    {
        private readonly List<Model.SapherStepData> stepData = new List<Model.SapherStepData>();

        public Task<Dtos.SapherStepData> GetStepInstanceFromInputMessageId(string stepName, string inputMessageId)
            => Task.FromResult(this.stepData
                .Find(sd =>
                    string.Equals(
                        sd.Id,
                        Model.SapherStepData.GenerateId(stepName, inputMessageId),
                        StringComparison.InvariantCultureIgnoreCase))
                ?.ToDto());

        public Task<Dtos.SapherStepData> GetStepInstanceFromOutputMessageId(string stepName, string outputMessageId)
            => Task.FromResult(this.stepData
                .Find(sd =>
                    string.Equals(
                        sd.StepName,
                        stepName,
                        StringComparison.InvariantCultureIgnoreCase)
                    && (sd.MessagesWaitingResponse.Contains(outputMessageId)
                        || sd.SuccessfulMessages.Contains(outputMessageId)
                        || sd.FailedMessages.Contains(outputMessageId)
                        || sd.CompensatedMessages.Contains(outputMessageId)))
                ?.ToDto());

        public Task<IEnumerable<Dtos.SapherStepData>> GetStepInstancesWaitingLonger(int timeoutInMinutes)
        {
            bool selector(SapherStepData instance)
                => instance.IsExpectingResponses
                && (DateTime.UtcNow.Subtract(instance.UpdateDate)).TotalMinutes > timeoutInMinutes;

            return Task.FromResult(this.stepData
                .Select(model => model.ToDto())
                .Where(dto => selector(dto)));
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
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member