namespace Sapher.Tests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Persistence;
    using Persistence.Repositories;

    [ExcludeFromCodeCoverage]
    internal class TestRepository : InMemorySapherRepository, ISapherDataRepository
    {
        internal static bool Executed { get; set; }

        public new Task<Dtos.SapherStepData> GetStepInstanceFromInputMessageId(string stepName, string inputMessageId)
        {
            Executed = true;
            return base.GetStepInstanceFromInputMessageId(stepName, inputMessageId);
        }

        public new Task<Dtos.SapherStepData> GetStepInstanceFromOutputMessageId(string stepName, string outputMessageId)
        {
            Executed = true;
            return base.GetStepInstanceFromOutputMessageId(stepName, outputMessageId);
        }

        public new Task<IEnumerable<Dtos.SapherStepData>> GetStepInstances(string stepName, int page, int pageSize)
        {
            Executed = true;
            return base.GetStepInstances(stepName, page, pageSize);
        }

        public new Task<IEnumerable<Dtos.SapherStepData>> GetStepInstancesWaitingLonger(int timeoutMs)
        {
            Executed = true;
            return base.GetStepInstancesWaitingLonger(timeoutMs);
        }

        public new Task Save(Dtos.SapherStepData data)
        {
            Executed = true;
            return base.Save(data);
        }
    }
}