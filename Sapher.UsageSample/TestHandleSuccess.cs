namespace TestConsole
{
    using System;
    using System.Threading.Tasks;
    using Sapher.Dtos;
    using Sapher.Handlers;

    public class TestSuccessMessage { }

    public class TestHandleSuccess : IHandlesResponse<TestSuccessMessage>
    {
        public bool HandlesFinalMessage { get; set; }

        public Task<ResponseResult> Execute(TestSuccessMessage message, object previouslyPersistedData)
        {
            Console.WriteLine("Executing TestSuccessMessage");
            var result = new ResponseResult
            {
                State = ResponseResultState.Successful,
                DataToPersist = previouslyPersistedData
            };

            return Task.FromResult(result);
        }
    }
}