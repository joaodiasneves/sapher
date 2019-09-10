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
                .AddRetryPolicy(expectedRetries, expectedRetryIntervalMs));

            //  .AddStep<TimeoutTestHandler>("TimeoutTest")
            //  .AddLogger<TestLogger>()
            //  .AddTimeoutPolicy(expectedTimeoutMs)
            //  .AddPersistence

            this.serviceProvider = this.serviceCollection.BuildServiceProvider();

            this.sapher = this.serviceProvider.UseSapher();
        }

        [Fact]
        public void UseSapher_RequireISapherFromDi_ReturnsTheSameObject()
        {
            // Act
            var sapherProvided = serviceProvider.GetRequiredService<ISapher>();

            // Assert
            sapherProvided.Should().BeEquivalentTo(this.sapher);
        }

        [Fact]
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
        }

        [Fact]
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
        }

        [Fact]
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
        }

        [Fact]
        public async Task DeliverMessage_ExceptionThrown_DeliveryResultIsFailedAndContainsException()
        {
            // Arrange
            var expectedExceptionMessage = "TestException";
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
        public async Task DeliverMessage_SapherExceptionThrown_DeliveryResultIsFailedAndContainsSapherException()
        {
            // Arrange
            var expectedExceptionMessage = "TestSapherException";
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
        public async Task DeliverMessage_SapherConfigurationExceptionThrown_DeliveryResultIsFailedAndContainsSapherConfigurationException()
        {
            // Arrange
            var expectedExceptionMessage = "TestException";
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

        // test persistence
        // .AddPersistence<TestRepository>()
    }
}