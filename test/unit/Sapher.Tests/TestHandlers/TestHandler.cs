namespace Sapher.Tests.TestHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Dtos;
    using Handlers;

    public class TestInputMessage
    {
        public int Value { get; set; }

        public string SimulatedOutputMessageId { get; set; }
    }

    public class TestSuccessMessage
    {
        public int TestValue { get; set; }
    }

    public class TestCompensationMessage { }

    public class TestFailureMessage { }

    public class TestDataObject
    {
        public int AnswerToEverything { get; set; }
    }

    public class TestHandler :
        IHandlesInput<TestInputMessage>,
        IHandlesResponse<TestSuccessMessage>,
        IHandlesResponse<TestCompensationMessage>,
        IHandlesResponse<TestFailureMessage>
    {
        public Task<InputResult> Execute(TestInputMessage message, MessageSlip messageSlip)
        {
            Console.WriteLine("Executing TestInputMessage");
            return Task.FromResult(new InputResult
            {
                OutputMessagesIds = new List<string> { message.SimulatedOutputMessageId },
                DataToPersist = new Dictionary<string, string>
                {
                    { "AnswerToEverything", message.Value.ToString() }
                },
                State = InputResultState.Successful
            });
        }

        public Task<ResponseResult> Execute(
            TestSuccessMessage message,
            MessageSlip messageSlip,
            IDictionary<string, string> previouslyPersistedData)
        {
            Console.WriteLine("Executing TestSuccessMessage");
            previouslyPersistedData["AnswerToEverything"] = message.TestValue.ToString();

            var result = new ResponseResult
            {
                State = ResponseResultState.Successful,
                DataToPersist = previouslyPersistedData
            };

            return Task.FromResult(result);
        }

        public Task<ResponseResult> Execute(
            TestCompensationMessage message,
            MessageSlip messageSlip,
            IDictionary<string, string> previouslyPersistedData)
        {
            Console.WriteLine("Compensating TestCompensationMessage");
            var result = new ResponseResult
            {
                State = ResponseResultState.Compensated,
                DataToPersist = previouslyPersistedData
            };

            return Task.FromResult(result);
        }

        public Task<ResponseResult> Execute(
            TestFailureMessage message,
            MessageSlip messageSlip,
            IDictionary<string, string> previouslyPersistedData)
        {
            Console.WriteLine("Executing TestFailureMessage");
            var result = new ResponseResult
            {
                State = ResponseResultState.Failed,
                DataToPersist = previouslyPersistedData
            };

            return Task.FromResult(result);
        }
    }
}