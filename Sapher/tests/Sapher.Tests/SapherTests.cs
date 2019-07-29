namespace Sapher.Tests
{
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

        public SapherTests()
        {
            this.serviceCollection = new ServiceCollection();
            serviceCollection.AddSapher(sapherConfig => sapherConfig
                .AddStep("TestStep", typeof(TestHandleInput), stepConfig => stepConfig
                    .AddResponseHandler(typeof(TestHandleSuccess))
                    .AddResponseHandler(typeof(TestHandleCompensation)))
                .AddStep("TestStep1", typeof(TestHandleInput), stepConfig => stepConfig
                    .AddResponseHandler(typeof(TestHandleSuccess))));
        }

        [Fact]
        public void UseSapher_RegistersISapherInDI_ReturnsTheSameObject()
        {
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var sapherUse = serviceProvider.UseSapher();
            var sapherProvided = serviceProvider.GetRequiredService<ISapher>();

            sapherUse.Should().BeEquivalentTo(sapherProvided);
        }

        [Fact]
        public async Task Deliver_DeliverMessage_Successful()
        {
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var sapher = serviceProvider.UseSapher();
            var expectedDataPersisted = 50;

            var deliveryResult = await sapher
                .DeliverMessage(
                    new TestInputMessage()
                    {
                        AnswerToEverything = expectedDataPersisted
                    },
                    Dtos.MessageSlip.GenerateNewMessageSlip())
                .ConfigureAwait(false);

            Assert.NotNull(deliveryResult);
            Assert.Single(deliveryResult.StepsExecuted);
            var stepExecuted = deliveryResult.StepsExecuted.Single();
            Assert.NotNull(stepExecuted.InputHandlerResult);
            Assert.Null(stepExecuted.ResponseHandlerResult);

            Assert.Equal(
                expectedDataPersisted,
                ((TestObject)stepExecuted.InputHandlerResult.DataToPersist).Life);
        }
    }
}