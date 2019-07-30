namespace Sapher.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Extensions;
    using FluentAssertions;
    using Microsoft.Extensions.DependencyInjection;
    using TestHandlers;
    using Xunit;

    public class SapherTests
    {
        private readonly ServiceCollection serviceCollection;
        private readonly ServiceProvider serviceProvider;
        private readonly ISapher sapher;

        public SapherTests()
        {
            // Arrange
            this.serviceCollection = new ServiceCollection();
            this.serviceCollection.AddSapher(sapherConfig => sapherConfig
                .AddStep("TestInputWithResponses", typeof(TestHandler), stepConfig => stepConfig
                    .AddResponseHandler(typeof(TestHandler))
                    .AddResponseHandler(typeof(TestHandler)))
                .AddStep("TestOnlyInput", typeof(TestHandler)));

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
            var expectedOutputId = "ccbba632-3c3a-40c0-8d6f-47c7d039c410";

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

                var dataToPersistCasted = ((TestDataObject)stepExecuted.InputHandlerResult.DataToPersist);
                Assert.Equal(expectedDataPersisted, dataToPersistCasted.AnswerToEverything);
                Assert.Single(stepExecuted.InputHandlerResult.OutputMessagesIds);
                var outputId = stepExecuted.InputHandlerResult.OutputMessagesIds.Single();
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
            var expectedOutputId = "ccbba632-3c3a-40c0-8d6f-47c7d039c410";
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

            var responseMessageSlip = new Dtos.MessageSlip
            {
                MessageId = Guid.NewGuid().ToString(),
                ConversationId = expectedOutputId,
                CorrelationId = inputMessageSlip.CorrelationId
            };

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
            Assert.Equal(expectedDataPersisted, stepExecuted.ResponseHandlerResult.DataToPersist);
        }
    }
}