namespace Sapher.Persistence
{
    using Model;

    public interface ISapherDataRepository
    {
        SapherStepData Load(string stepName, string inputMessageId);

        SapherStepData Load(string id);

        SapherStepData LoadFromConversationId(string id, string outputMessageId);

        void Save(SapherStepData data);
    }
}