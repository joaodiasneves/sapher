namespace TestConsole
{
    using System;
    using System.Threading.Tasks;
    using Sapher.Dtos;
    using Sapher.Handlers;
    using Sapher.Persistence.Model;

    public class TestSuccessMessage { }

    public class TestHandleSuccess : IHandlesSuccess<TestSuccessMessage>
    {
        public bool HandlesFinalMessage { get; set; }

        public Task<Result> Execute(TestSuccessMessage message, SapherStepData data)
        {
            Console.WriteLine("Executing TestSuccessMessage");
            var result = new Result
            {
                IsSuccess = true,
                DataToPersist = data.DataToPersist
            };

            return Task.FromResult(result);
        }
    }
}