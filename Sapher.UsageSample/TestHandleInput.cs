namespace UsageSample
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Sapher.Dtos;
    using Sapher.Handlers;

    public class TestInputMessage { }

    public class TestHandleInput : IHandlesInput<TestInputMessage>
    {
        public Task<InputResult> Execute(TestInputMessage message, MessageSlip messageSlip)
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