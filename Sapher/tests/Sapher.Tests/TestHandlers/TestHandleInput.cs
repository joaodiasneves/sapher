namespace Sapher.Tests.TestHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Dtos;
    using Handlers;

    public class TestInputMessage { }

    public class TestHandleInput : IHandlesInput<TestInputMessage>
    {
        public Task<InputResult> Execute(TestInputMessage message)
        {
            Console.WriteLine("Executing TestInputMessage");
            return Task.FromResult(new InputResult
            {
                OutputMessagesIds = new List<string> { Guid.NewGuid().ToString() },
                DataToPersist = new { life = 42 },
                State = InputResultState.Successful
            });
        }
    }
}