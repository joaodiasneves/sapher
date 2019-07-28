namespace Sapher.Tests.TestHandlers
{
    using System;
    using System.Threading.Tasks;
    using Dtos;
    using Handlers;
    using Persistence.Model;

    public class TestCompensationMessage { }

    public class TestHandleCompensation : IHandlesResponse<TestCompensationMessage>
    {
        public Task<ResponseResult> Execute(TestCompensationMessage message, SapherStepData data)
        {
            Console.WriteLine("Executing TestCompensationMessage");
            var result = new ResponseResult
            {
                DataToPersist = data.DataToPersist,
                State = Dtos.ResponseResultState.Compensated
            };

            return Task.FromResult(result);
        }
    }
}