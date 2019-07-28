namespace Sapher.Handlers
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;

    public static class HandlersFactory
    {
        public static bool TryToRegisterInputHandler(
            Type inputHandlerType,
            IServiceCollection serviceCollection,
            out Type messageType,
            out string outputMessage)
        {
            return TryToRegisterHandler(
                typeof(IHandlesInput<>),
                inputHandlerType,
                serviceCollection,
                out messageType,
                out outputMessage);
        }

        public static bool TryToRegisterResponseHandler(
            Type responseHandlerType,
            IServiceCollection serviceCollection,
            out Type messageType,
            out string outputMessage)
        {
            return TryToRegisterHandler(
                typeof(IHandlesResponse<>),
                responseHandlerType,
                serviceCollection,
                out messageType,
                out outputMessage);
        }

        private static bool TryToRegisterHandler(
            Type sapherHandlerType,
            Type implementationHandlerType,
            IServiceCollection serviceCollection,
            out Type messageType,
            out string outputMessage)
        {
            messageType = default(Type);
            outputMessage = "Handler instantiated.";

            var expectedType = sapherHandlerType;
            var implementedInterface = implementationHandlerType
                .GetInterfaces()
                .FirstOrDefault(i => 
                    i.IsGenericType 
                    && i.GetGenericTypeDefinition() == expectedType);

            if (implementedInterface == null)
            {
                outputMessage =
                    $"Expected implementation of generic {expectedType.Name}, " +
                    $"but received {implementationHandlerType.Name}";
                return false;
            }

            if (implementationHandlerType.IsGenericType)
            {
                outputMessage = $"{implementationHandlerType.Name} can not be a Generic definition";
                return false;
            }

            messageType = implementedInterface.GenericTypeArguments[0];
            serviceCollection.AddTransient(implementedInterface, implementationHandlerType);
            return true;
        }
    }
}