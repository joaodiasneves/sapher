namespace TestConsole
{
    using System;
    using System.Threading.Tasks;
    using Sapher.Dtos;
    using Sapher.Handlers;
    using Sapher.Persistence.Model;

    public class TestSuccessMessage { }

    public class TestHandleSuccess : IHandlesResponse<TestSuccessMessage>
    {
        public bool HandlesFinalMessage { get; set; }

        public Task<ResponseResult> Execute(TestSuccessMessage message, SapherStepData data)
        {
            Console.WriteLine("Executing TestSuccessMessage");
            var result = new ResponseResult
            {
                State = Sapher.Dtos.ResponseResultState.Successful,
                DataToPersist = data.DataToPersist
            };

            return Task.FromResult(result);
        }
    }
}