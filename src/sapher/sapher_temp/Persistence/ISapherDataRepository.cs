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
        /// Finds all instances of the step specified by <paramref name="stepName"/>. Paginated with <paramref name="page"/> and <paramref name="pageSize"/>
        /// </summary>
        /// <param name="stepName">The name of the step to read</param>
        /// <param name="page">The number of the page to retrieve.</param>
        /// <param name="pageSize">The number of items to retrieve per page.</param>
        /// <returns>Sapher information regarding the mentioned step</returns>
        Task<IEnumerable<Dtos.SapherStepData>> GetStepInstances(string stepName, int page, int pageSize);

        /// <summary>
        /// Identifies all the SapherStepData instances that are still waiting for responses for longer than the time specified in <paramref name="timeoutMs"/>
        /// </summary>
        /// <param name="timeoutMs">Time in milliseconds to wait before considering execution timed out</param>
        /// <returns>The identified SapherStepData instances</returns>
        /// <remarks>SapherStepData.UpdateDate should be used to evaluate the time difference</remarks>
        Task<IEnumerable<Dtos.SapherStepData>> GetStepInstancesWaitingLonger(int timeoutMs);

        /// <summary>
        /// Upserts an instance of SapherStepData by the one provided by <paramref name="data"/>
        /// </summary>
        /// <param name="data">SapherStepData instance to be upserted</param>
        Task Save(SapherStepData data);
    }
}