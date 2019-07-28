namespace TestConsole
{
    using System;
    using System.Threading.Tasks;
    using Sapher.Dtos;
    using Sapher.Handlers;
    using Sapher.Persistence.Model;

    public class TestCompensationMessage { }

    public class TestHandleCompensation : IHandlesResponse<TestCompensationMessage>
    {
        public Task<ResponseResult> Execute(TestCompensationMessage message, SapherStepData data)
        {
            Console.WriteLine("Executing TestCompensationMessage");
            var result = new ResponseResult
            {
                DataToPersist = data.DataToPersist,
                State = Sapher.Dtos.ResponseResultState.Compensated
            };

            return Task.FromResult(result);
        }
    }
}