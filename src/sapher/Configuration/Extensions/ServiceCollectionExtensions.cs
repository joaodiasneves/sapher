namespace Sapher.Configuration.Extensions
{
    using System;
    using Configuration;
    using Configuration.Implementations;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static ISapher UseSapher(this IServiceProvider serviceProvider)
        {
            var sapher = serviceProvider.GetRequiredService<ISapher>();
            sapher.Init(serviceProvider);
            return sapher;
        }

        public static IServiceCollection AddSapher(
            this IServiceCollection serviceCollection,
            Action<ISapherConfigurator> configure)
        {
            var sapherConfigurator = new SapherConfigurator(serviceCollection);
            configure(sapherConfigurator);
            var sapherConfiguration = sapherConfigurator.Configure();

            var sapher = new Sapher(sapherConfiguration);
            serviceCollection.AddSingleton<ISapher>(sapher);

            return serviceCollection;
        }
    }
}