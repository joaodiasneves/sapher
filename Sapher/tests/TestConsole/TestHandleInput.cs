namespace TestConsole
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Sapher.Dtos;
    using Sapher.Handlers;

    public class TestInputMessage { }

    public class TestHandleInput : IHandlesStepInput<TestInputMessage>
    {
        public Task<InputResult> Execute(TestInputMessage message)
        {
            Console.WriteLine("Executing TestInputMessage");
            return Task.FromResult(new InputResult
            {
                IsSuccess = true,
                OutputMessagesIds = new List<string> { Guid.NewGuid().ToString() },
                DataToPersist = new { life = 42 }
            });
        }
    }
}