namespace Sapher.Configuration.Extensions
{
    using System;
    using Configuration;
    using Configuration.Implementations;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Provides <c>Microsoft.Extensions.DependencyInjection</c> extensions to use Sapher
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Initializes Sapher as previously configured
        /// </summary>
        /// <returns>Sapher instance</returns>
        public static ISapher UseSapher(this IServiceProvider serviceProvider)
        {
            var sapher = serviceProvider.GetRequiredService<IInternalSapher>();
            sapher.Init(serviceProvider);
            return sapher;
        }

        /// <summary>
        /// Adds Sapher to <see cref="IServiceCollection"/> using the defined configurations
        /// </summary>
        /// <returns>ServiceCollection for fluent use</returns>
        public static IServiceCollection AddSapher(
            this IServiceCollection serviceCollection,
            Action<ISapherConfigurator> configure)
        {
            var sapherConfigurator = new SapherConfigurator(serviceCollection);
            configure(sapherConfigurator);
            var sapherConfiguration = sapherConfigurator.Configure();

            var sapher = new Sapherman(sapherConfiguration);
            serviceCollection.AddSingleton<ISapher>(sapher);
            serviceCollection.AddSingleton<IInternalSapher>(sapher);

            return serviceCollection;
        }
    }
}