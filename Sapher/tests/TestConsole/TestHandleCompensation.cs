namespace TestConsole
{
    using System;
    using System.Threading.Tasks;
    using Sapher.Dtos;
    using Sapher.Handlers;
    using Sapher.Persistence.Model;

    public class TestCompensationMessage { }

    public class TestHandleCompensation : IHandlesCompensation<TestCompensationMessage>
    {
        public Task<Result> Compensate(TestCompensationMessage message, SapherStepData sapherData)
        {
            Console.WriteLine("Executing TestCompensationMessage");
            var result = new Result
            {
                DataToPersist = sapherData.DataToPersist,
                IsSuccess = true
            };

            return Task.FromResult(result);
        }
    }
}