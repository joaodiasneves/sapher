namespace Sapher.Tests.TestHandlers
{
    using System;
    using System.Threading.Tasks;
    using Dtos;
    using Handlers;

    public class TestSuccessMessage { }

    public class TestHandleSuccess : IHandlesResponse<TestSuccessMessage>
    {
        public bool HandlesFinalMessage { get; set; }

        public Task<ResponseResult> Execute(TestSuccessMessage message, object previouslyPersistedData)
        {
            Console.WriteLine("Executing TestSuccessMessage");
            var result = new ResponseResult
            {
                State = Dtos.ResponseResultState.Successful,
                DataToPersist = previouslyPersistedData
            };

            return Task.FromResult(result);
        }
    }
}