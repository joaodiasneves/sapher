namespace Sapher.Tests
{
    using Microsoft.Extensions.DependencyInjection;
    using Extensions;
    using Xunit;
    using TestHandlers;
    using FluentAssertions;

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
    }
}
