namespace Sapher.Persistence
{
    using System;
    using System.Threading.Tasks;
    using Dtos;

    public interface ISapherDataRepository
    {
        Task<SapherStepData> Load(string stepName, string inputMessageId);

        Task<SapherStepData> LoadFromConversationId(string stepName, string outputMessageId);

        Task UpdateInstancesState(Func<SapherStepData, bool> selector, StepState stepState);

        Task<bool> Save(SapherStepData data);
    }
}