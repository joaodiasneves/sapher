namespace UsageSample
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Sapher.Dtos;
    using Sapher.Handlers;

    public class TestCompensationMessage { }

    public class TestHandleCompensation : IHandlesResponse<TestCompensationMessage>
    {
        public Task<ResponseResult> Execute(TestCompensationMessage message, MessageSlip messageSlip, IDictionary<string, string> previouslyPersistedData)
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