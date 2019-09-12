namespace Sapher.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using Configuration.Extensions;
    using FluentAssertions;
    using Handlers;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class SapherTests
    {
        private readonly ServiceCollection serviceCollection;
        private readonly ServiceProvider serviceProvider;
        private readonly ISapher sapher;
        private const int expectedRetries = 3;
        private const int expectedRetryIntervalMs = 1;
        private const int expectedTimeoutMs = 500;

        public SapherTests()
        {
            // Arrange
            this.serviceCollection = new ServiceCollection();
            this.serviceCollection.AddSapher(sapherConfig => sapherConfig
                .AddStep<TestHandler>("TestInputWithResponses", stepConfig => stepConfig
                    .AddResponseHandler<TestHandler>()
                    .AddResponseHandler<TestHandler>())
                .AddStep<TestHandler>("TestIfBothStepsAreExecuted")
                .AddStep<RetryExceptionTestHandler>("RetryExceptionTest")
                .AddStep<RetrySapherExceptionTestHandler>("RetrySapherExceptionTest")
                .AddStep<RetrySapherConfigurationExceptionTestHandler>("RetrySapherConfigurationExceptionTest")
                .AddLogger<TestLogger>()
                .AddRetryPolicy(expectedRetries, expectedRetryIntervalMs)
                .AddTimeoutPolicy(expectedTimeoutMs)
                .AddPersistence<TestRepository>());

            this.serviceProvider = this.serviceCollection.BuildServiceProvider();

            this.sapher = this.serviceProvider.UseSapher();
        }

        [Fact]
        [Trait("Category", "SapherSetup")]
        public void UseSapher_RequireISapherFromDi_ReturnsTheSameObject()
        {
            // Act
            var sapherProvided = serviceProvider.GetRequiredService<ISapher>();

            // Assert
            sapherProvided.Should().BeEquivalentTo(this.sapher);
        }

        [Fact]
        [Trait("Category", "DeliverMessage")]
        [Trait("Category", "UnitTest")]
        public async Task DeliverMessage_InputOfTwoSteps_TwoStepResultsWithResponseResultNull()
        {
            // Arrange
            const int expectedDataPersisted = 50;
            const int expectedStepsCount = 2;
            const string expectedOutputId = "ccbba632-3c3a-40c0-8d6f-47c7d039c410";

            // Act
            var deliveryResult = await this.sapher
                .DeliverMessage(
                    new TestInputMessage
                    {
                        Value = expectedDataPersisted,
                        SimulatedOutputMessageId = expectedOutputId
                    },
                    Dtos.MessageSlip.GenerateNewMessageSlip())
                .ConfigureAwait(false);

            // Assert
            Assert.NotNull(deliveryResult);
            Assert.NotNull(deliveryResult.StepsExecuted);
            Assert.Equal(expectedStepsCount, deliveryResult.StepsExecuted.Count());

            foreach (var stepExecuted in deliveryResult.StepsExecuted)
            {
                Assert.NotNull(stepExecuted.InputHandlerResult);
                Assert.Null(stepExecuted.ResponseHandlerResult);

                Assert.Equal(expectedDataPersisted, int.Parse(stepExecuted.InputHandlerResult.DataToPersist["AnswerToEverything"]));
                Assert.Single(stepExecuted.InputHandlerResult.SentMessageIds);
                var outputId = stepExecuted.InputHandlerResult.SentMessageIds.Single();
                Assert.Equal(expectedOutputId, outputId);
                Assert.Equal(Dtos.InputResultState.Successful, stepExecuted.InputHandlerResult.State);
            }

            Assert.True(TestRepository.Executed);
            TestRepository.Executed = false;
        }

        [Fact]
        [Trait("Category", "DeliverMessage")]
        [Trait("Category", "UnitTest")]
        public async Task DeliverMessage_ValidResponseMessageBeforeInput_OneStepResultWithResultsNull()
        {
            // Act
            var deliveryResult = await this.sapher
                .DeliverMessage(
                    new TestSuccessMessage(),
                    Dtos.MessageSlip.GenerateNewMessageSlip())
                .ConfigureAwait(false);

            // Assert
            Assert.NotNull(deliveryResult);
            Assert.NotNull(deliveryResult.StepsExecuted);
            Assert.Single(deliveryResult.StepsExecuted);

            foreach (var stepExecuted in deliveryResult.StepsExecuted)
            {
                Assert.Null(stepExecuted.InputHandlerResult);
                Assert.Null(stepExecuted.ResponseHandlerResult);
            }

            Assert.True(TestRepository.Executed);
            TestRepository.Executed = false;
        }

        [Fact]
        [Trait("Category", "DeliverMessage")]
        [Trait("Category", "UnitTest")]
        public async Task DeliverMessage_ResponseMessageAfterInput_OneStepResultWithValidResponses()
        {
            // Arrange
            const int expectedDataPersisted = 50;
            const string expectedOutputId = "ccbba632-3c3a-40c0-8d6f-47c7d039c410";
            var inputMessageSlip = Dtos.MessageSlip.GenerateNewMessageSlip();

            await this.sapher
                .DeliverMessage(
                    new TestInputMessage
                    {
                        Value = 42,
                        SimulatedOutputMessageId = expectedOutputId
                    },
                    inputMessageSlip)
                .ConfigureAwait(false);

            var responseMessageSlip = new Dtos.MessageSlip(
                Guid.NewGuid().ToString(),
                expectedOutputId,
                inputMessageSlip.CorrelationId);

            // Act
            var deliveryResult = await this.sapher
                .DeliverMessage(
                    new TestSuccessMessage
                    {
                        TestValue = expectedDataPersisted
                    },
                    responseMessageSlip)
                .ConfigureAwait(false);

            // Assert
            Assert.NotNull(deliveryResult);
            Assert.NotNull(deliveryResult.StepsExecuted);
            Assert.Single(deliveryResult.StepsExecuted);
            var stepExecuted = deliveryResult.StepsExecuted.Single();
            Assert.Null(stepExecuted.InputHandlerResult);
            Assert.NotNull(stepExecuted.ResponseHandlerResult);
            Assert.Equal(Dtos.ResponseResultState.Successful, stepExecuted.ResponseHandlerResult.State);
            Assert.Equal(expectedDataPersisted, int.Parse(stepExecuted.ResponseHandlerResult.DataToPersist["AnswerToEverything"]));

            Assert.True(TestRepository.Executed);
            TestRepository.Executed = false;
        }

        [Fact]
        [Trait("Category", "DeliverMessage")]
        [Trait("Category", "UnitTest")]
        public async Task DeliverMessage_ExceptionThrown_DeliveryResultIsFailedAndContainsException()
        {
            // Arrange
            const string expectedExceptionMessage = "TestException";
            var expectedException = new Exception(expectedExceptionMessage);
            var message = new ExceptionMessage
            {
                ExpectedException = expectedException
            };

            var inputMessageSlip = Dtos.MessageSlip.GenerateNewMessageSlip();

            // Act
            var deliveryResult = await this.sapher
                .DeliverMessage(
                    message,
                    inputMessageSlip)
                .ConfigureAwait(false);

            // Assert
            Assert.NotNull(deliveryResult);
            Assert.NotNull(deliveryResult.StepsExecuted);
            Assert.Empty(deliveryResult.StepsExecuted);
            Assert.Equal(expectedRetries, RetryExceptionTestHandler.ExecutionCount);
            Assert.True(deliveryResult.IsDeliveryFailed);
            Assert.NotNull(deliveryResult.ErrorMessage);
            Assert.Equal(expectedExceptionMessage, deliveryResult.ErrorMessage);
            Assert.NotNull(deliveryResult.Exception);
            Assert.Equal(expectedException, deliveryResult.Exception);
        }

        [Fact]
        [Trait("Category", "DeliverMessage")]
        [Trait("Category", "UnitTest")]
        public async Task DeliverMessage_SapherExceptionThrown_DeliveryResultIsFailedAndContainsSapherException()
        {
            // Arrange
            const string expectedExceptionMessage = "TestSapherException";
            var expectedException = new Exceptions.SapherException(expectedExceptionMessage);
            var message = new SapherExceptionMessage
            {
                ExpectedException = expectedException
            };

            var inputMessageSlip = Dtos.MessageSlip.GenerateNewMessageSlip();

            // Act
            var deliveryResult = await this.sapher
                .DeliverMessage(
                    message,
                    inputMessageSlip)
                .ConfigureAwait(false);

            // Assert
            Assert.NotNull(deliveryResult);
            Assert.NotNull(deliveryResult.StepsExecuted);
            Assert.Empty(deliveryResult.StepsExecuted);
            Assert.Equal(expectedRetries, RetrySapherExceptionTestHandler.ExecutionCount);
            Assert.True(deliveryResult.IsDeliveryFailed);
            Assert.NotNull(deliveryResult.ErrorMessage);
            Assert.Equal(expectedExceptionMessage, deliveryResult.ErrorMessage);
            Assert.NotNull(deliveryResult.Exception);
            Assert.Equal(expectedException, deliveryResult.Exception);
        }

        [Fact]
        [Trait("Category", "DeliverMessage")]
        [Trait("Category", "UnitTest")]
        public async Task DeliverMessage_SapherConfigurationExceptionThrown_DeliveryResultIsFailedAndContainsSapherConfigurationException()
        {
            // Arrange
            const string expectedExceptionMessage = "TestException";
            var expectedException = new Exceptions.SapherConfigurationException(expectedExceptionMessage);
            var message = new SapherConfigurationExceptionMessage
            {
                ExpectedException = expectedException
            };

            var inputMessageSlip = Dtos.MessageSlip.GenerateNewMessageSlip();

            // Act
            var deliveryResult = await this.sapher
                .DeliverMessage(
                    message,
                    inputMessageSlip)
                .ConfigureAwait(false);

            // Assert
            Assert.NotNull(deliveryResult);
            Assert.NotNull(deliveryResult.StepsExecuted);
            Assert.Empty(deliveryResult.StepsExecuted);
            Assert.Equal(expectedRetries, RetrySapherConfigurationExceptionTestHandler.ExecutionCount);
            Assert.True(deliveryResult.IsDeliveryFailed);
            Assert.NotNull(deliveryResult.ErrorMessage);
            Assert.Equal(expectedExceptionMessage, deliveryResult.ErrorMessage);
            Assert.NotNull(deliveryResult.Exception);
            Assert.Equal(expectedException, deliveryResult.Exception);
        }

        [Fact]
        [Trait("Category", "DeliverMessage")]
        [Trait("Category", "UnitTest")]
        public async Task DeliverMessage_ExceptionThrown_ExceptionLoggedAsError()
        {
            // Arrange
            const string expectedExceptionMessage = "TestException";
            var expectedException = new Exception(expectedExceptionMessage);
            var message = new ExceptionMessage
            {
                ExpectedException = expectedException
            };

            const Logger.LoggingEventType expectedLogLevel = Logger.LoggingEventType.Error;

            var inputMessageSlip = Dtos.MessageSlip.GenerateNewMessageSlip();

            // Act
            var deliveryResult = await this.sapher
                .DeliverMessage(
                    message,
                    inputMessageSlip)
                .ConfigureAwait(false);

            // Assert
            Assert.NotNull(deliveryResult.Exception);
            Assert.Equal(deliveryResult.Exception, TestLogger.ReceivedLogEntry.Exception);
            Assert.Equal(expectedException, TestLogger.ReceivedLogEntry.Exception);
            Assert.Equal(expectedExceptionMessage, TestLogger.ReceivedLogEntry.Message);
            Assert.Equal(expectedLogLevel, TestLogger.ReceivedLogEntry.Severity);
        }

        [Fact]
        [Trait("Category", "DeliverMessage")]
        [Trait("Category", "UnitTest")]
        public async Task DeliverMessage_SapherExceptionThrown_ExceptionLoggedAsWarning()
        {
            // Arrange
            const string expectedExceptionMessage = "TestSapherException";
            var expectedException = new Exceptions.SapherException(expectedExceptionMessage);
            var message = new SapherExceptionMessage
            {
                ExpectedException = expectedException
            };

            const Logger.LoggingEventType expectedLogLevel = Logger.LoggingEventType.Warning;

            var inputMessageSlip = Dtos.MessageSlip.GenerateNewMessageSlip();

            // Act
            var deliveryResult = await this.sapher
                .DeliverMessage(
                    message,
                    inputMessageSlip)
                .ConfigureAwait(false);

            // Assert
            Assert.NotNull(deliveryResult.Exception);
            Assert.Equal(deliveryResult.Exception, TestLogger.ReceivedLogEntry.Exception);
            Assert.Equal(expectedException, TestLogger.ReceivedLogEntry.Exception);
            Assert.Equal(expectedExceptionMessage, TestLogger.ReceivedLogEntry.Message);
            Assert.Equal(expectedLogLevel, TestLogger.ReceivedLogEntry.Severity);
        }

        [Fact]
        [Trait("Category", "DeliverMessage")]
        [Trait("Category", "UnitTest")]
        public async Task GetStepInstance_RetrievesInstanceValidInfo()
        {
            // Arrange
            var expectedOutputMessageId = Guid.NewGuid().ToString();
            const int expectedValue = 42;
            var message = new TestInputMessage
            {
                SimulatedOutputMessageId = expectedOutputMessageId,
                Value = expectedValue
            };

            var inputMessageSlip = Dtos.MessageSlip.GenerateNewMessageSlip();
            const string  expectedStepName = "TestInputWithResponses";

            await this.sapher
               .DeliverMessage(
                   message,
                   inputMessageSlip,
                   expectedStepName)
               .ConfigureAwait(false);

            // Act
            var dataRetrieved = await this.sapher.GetStepInstance(expectedStepName, inputMessageSlip.MessageId).ConfigureAwait(false);

            // Assert
            Assert.Equal(Dtos.StepState.ExecutedInput, dataRetrieved.State);
            Assert.Equal(expectedValue.ToString(), dataRetrieved.DataToPersist["AnswerToEverything"]);
            Assert.Single(dataRetrieved.MessagesWaitingResponse);
            Assert.Equal(expectedOutputMessageId, dataRetrieved.MessagesWaitingResponse.Single());
            Assert.Equal(inputMessageSlip.MessageId, dataRetrieved.InputMessageSlip.MessageId);
            Assert.Equal(inputMessageSlip.ConversationId, dataRetrieved.InputMessageSlip.ConversationId);
            Assert.Equal(inputMessageSlip.CorrelationId, dataRetrieved.InputMessageSlip.CorrelationId);
            Assert.Equal(expectedStepName, dataRetrieved.StepName);
            Assert.Empty(dataRetrieved.CompensatedMessages);
            Assert.Empty(dataRetrieved.FailedMessages);
            Assert.Empty(dataRetrieved.SuccessfulMessages);

            Assert.True(TestRepository.Executed);
            TestRepository.Executed = false;
        }

        [Fact]
        [Trait("Category", "DeliverMessage")]
        [Trait("Category", "UnitTest")]
        public async Task GetStepInstances_RetrievesInstancesValidInfo()
        {
            // Arrange
            var expectedOutputMessageId = Guid.NewGuid().ToString();
            const int expectedValue = 42;

            var inputMessageSlip1 = Dtos.MessageSlip.GenerateNewMessageSlip();
            var inputMessageSlip2 = Dtos.MessageSlip.GenerateNewMessageSlip();

            var message1 = new TestInputMessage
            {
                SimulatedOutputMessageId = expectedOutputMessageId,
                Value = expectedValue
            };

            var message2 = new TestInputMessage
            {
                SimulatedOutputMessageId = expectedOutputMessageId,
                Value = expectedValue
            };

            const string expectedStepName = "TestInputWithResponses";

            await this.sapher
               .DeliverMessage(
                   message1,
                   inputMessageSlip1,
                   expectedStepName)
               .ConfigureAwait(false);

            await this.sapher
              .DeliverMessage(
                  message2,
                  inputMessageSlip2,
                  expectedStepName)
              .ConfigureAwait(false);

            // Act
            var dataRetrieved = await this.sapher.GetStepInstances(expectedStepName).ConfigureAwait(false);

            // Assert
            var dataRetrieved1 = dataRetrieved.First();
            Assert.Equal(Dtos.StepState.ExecutedInput, dataRetrieved1.State);
            Assert.Equal(expectedValue.ToString(), dataRetrieved1.DataToPersist["AnswerToEverything"]);
            Assert.Single(dataRetrieved1.MessagesWaitingResponse);
            Assert.Equal(expectedOutputMessageId, dataRetrieved1.MessagesWaitingResponse.Single());
            Assert.Equal(inputMessageSlip1.MessageId, dataRetrieved1.InputMessageSlip.MessageId);
            Assert.Equal(inputMessageSlip1.ConversationId, dataRetrieved1.InputMessageSlip.ConversationId);
            Assert.Equal(inputMessageSlip1.CorrelationId, dataRetrieved1.InputMessageSlip.CorrelationId);
            Assert.Equal(expectedStepName, dataRetrieved1.StepName);
            Assert.Empty(dataRetrieved1.CompensatedMessages);
            Assert.Empty(dataRetrieved1.FailedMessages);
            Assert.Empty(dataRetrieved1.SuccessfulMessages);

            var dataRetrieved2 = dataRetrieved.Last();
            Assert.Equal(Dtos.StepState.ExecutedInput, dataRetrieved2.State);
            Assert.Equal(expectedValue.ToString(), dataRetrieved2.DataToPersist["AnswerToEverything"]);
            Assert.Single(dataRetrieved2.MessagesWaitingResponse);
            Assert.Equal(expectedOutputMessageId, dataRetrieved2.MessagesWaitingResponse.Single());
            Assert.Equal(inputMessageSlip2.MessageId, dataRetrieved2.InputMessageSlip.MessageId);
            Assert.Equal(inputMessageSlip2.ConversationId, dataRetrieved2.InputMessageSlip.ConversationId);
            Assert.Equal(inputMessageSlip2.CorrelationId, dataRetrieved2.InputMessageSlip.CorrelationId);
            Assert.Equal(expectedStepName, dataRetrieved2.StepName);
            Assert.Empty(dataRetrieved2.CompensatedMessages);
            Assert.Empty(dataRetrieved2.FailedMessages);
            Assert.Empty(dataRetrieved2.SuccessfulMessages);

            Assert.True(TestRepository.Executed);
            TestRepository.Executed = false;
        }
    }
}