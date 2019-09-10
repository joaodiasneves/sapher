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
        /// <summary>
        /// Finds the SapherStepData instance that has <paramref name="inputMessageId"/> as Input Message's MessageId,
        /// and the step name equals <paramref name="stepName"/>
        /// </summary>
        /// <param name="stepName">Name of the step to search</param>
        /// <param name="inputMessageId">SapherStepData Input message's id</param>
        /// <returns>The identified SapherStepData instance</returns>
        Task<Dtos.SapherStepData> GetStepInstanceFromInputMessageId(string stepName, string inputMessageId);

        /// <summary>
        /// Finds the SapherStepData instance that has <paramref name="outputMessageId"/> as an Output Message's MessageId,
        /// and the step name equals <paramref name="stepName"/>
        /// </summary>
        /// <param name="stepName">Name of the step to search</param>
        /// <param name="outputMessageId">Output message's id of the SapherStepData instance</param>
        /// <returns>The identified SapherStepData instance</returns>
        Task<Dtos.SapherStepData> GetStepInstanceFromOutputMessageId(string stepName, string outputMessageId);

        /// <summary>
        /// Identifies all the SapherStepData instances that are still waiting for responses for longer than the time specified
        /// in <paramref name="timeoutInMinutes"/>
        /// </summary>
        /// <param name="timeoutInMinutes"></param>
        /// <returns>The identified SapherStepData instances</returns>
        /// <remarks>SapherStepData.UpdateDate should be used to evaluate the time difference</remarks>
        Task<IEnumerable<Dtos.SapherStepData>> GetStepInstancesWaitingLonger(int timeoutInMinutes);

        /// <summary>
        /// Upserts an instance of SapherStepData by the one provided by <paramref name="data"/>
        /// </summary>
        /// <param name="data">SapherStepData instance to be upserted</param>
        /// <returns></returns>
        Task<bool> Save(SapherStepData data);
    }
}