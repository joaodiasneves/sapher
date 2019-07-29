namespace Sapher.Persistence
{
    using System.Threading.Tasks;
    using Model;

    public interface ISapherDataRepository
    {
        Task<SapherStepData> Load(string stepName, string inputMessageId);

        Task<SapherStepData> Load(string id);

        Task<SapherStepData> LoadFromConversationId(string stepName, string outputMessageId);

        Task<bool> Save(SapherStepData data);
    }
}