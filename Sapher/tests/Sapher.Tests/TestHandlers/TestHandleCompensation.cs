namespace Sapher.Tests.TestHandlers
{
    using System;
    using System.Threading.Tasks;
    using Dtos;
    using Handlers;

    public class TestCompensationMessage { }

    public class TestHandleCompensation : IHandlesResponse<TestCompensationMessage>
    {
        public Task<ResponseResult> Execute(TestCompensationMessage message, object previouslyPersistedData)
        {
            Console.WriteLine("Executing TestCompensationMessage");
            var result = new ResponseResult
            {
                DataToPersist = previouslyPersistedData,
                State = ResponseResultState.Compensated
            };

            return Task.FromResult(result);
        }
    }
}