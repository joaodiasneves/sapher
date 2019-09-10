namespace Sapher.Persistence
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Dtos;

    /// <summary>
    /// Repository to persist Sapher information regarding the state of the Distributed Transactions Steps executions.
    /// </summary>
    public interface ISapherDataRepository
    {
        Task<Dtos.SapherStepData> GetStepInstanceFromInputMessageId(string stepName, string inputMessageId);

        Task<Dtos.SapherStepData> GetStepInstanceFromOutputMessageId(string stepName, string outputMessageId);

        Task<IEnumerable<Dtos.SapherStepData>> GetStepInstancesWaitingLonger(int timeoutInMinutes);

        Task<bool> Save(SapherStepData data);
    }
}