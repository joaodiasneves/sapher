namespace Sapher.Tests.TestHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Dtos;
    using Handlers;

    public class TestInputMessage
    {
        public int AnswerToEverything { get; set; } = 42;
    }

    public class TestObject
    {
        public int Life { get; set; }
    }

    public class TestHandleInput : IHandlesInput<TestInputMessage>
    {
        public Task<InputResult> Execute(TestInputMessage message)
        {
            Console.WriteLine("Executing TestInputMessage");
            return Task.FromResult(new InputResult
            {
                OutputMessagesIds = new List<string> { Guid.NewGuid().ToString() },
                DataToPersist = new TestObject { Life = message.AnswerToEverything },
                State = InputResultState.Successful
            });
        }
    }
}