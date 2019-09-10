namespace Sapher.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Persistence;

    internal class TestRepository : ISapherDataRepository
    {
        internal bool Executed { get; set; }

        public Task<Dtos.SapherStepData> GetStepInstanceFromInputMessageId(string stepName, string inputMessageId)
        {
            this.Executed = true;
            return Task.FromResult((Dtos.SapherStepData)null);
        }

        public Task<Dtos.SapherStepData> GetStepInstanceFromOutputMessageId(string stepName, string outputMessageId)
        {
            this.Executed = true;
            return Task.FromResult((Dtos.SapherStepData)null);
        }

        public Task<IEnumerable<Dtos.SapherStepData>> GetStepInstancesWaitingLonger(int timeoutMs)
        {
            this.Executed = true;
            return Task.FromResult((IEnumerable<Dtos.SapherStepData>)null);
        }

        public Task Save(Dtos.SapherStepData data)
        {
            this.Executed = true;
            return Task.CompletedTask;
        }
    }
}