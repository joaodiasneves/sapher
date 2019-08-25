namespace Sapher.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;

    public static class HandlersFactory
    {
        public static bool TryToRegisterInputHandler(
            Type inputHandlerType,
            IServiceCollection serviceCollection,
            out Type messageType,
            out string outputMessage)
        {
            var result = TryToRegisterHandler(
                typeof(IHandlesInput<>),
                inputHandlerType,
                serviceCollection,
                false,
                out var messageTypes,
                out outputMessage);

            messageType = messageTypes.Single();
            return result;
        }

        public static bool TryToRegisterResponseHandler(
            Type responseHandlerType,
            IServiceCollection serviceCollection,
            out IList<Type> messageTypes,
            out string outputMessage)
        {
            return TryToRegisterHandler(
                typeof(IHandlesResponse<>),
                responseHandlerType,
                serviceCollection,
                true,
                out messageTypes,
                out outputMessage);
        }

        private static bool TryToRegisterHandler(
            Type sapherHandlerType,
            Type implementationHandlerType,
            IServiceCollection serviceCollection,
            bool allowMultipleHandlers,
            out IList<Type> messageTypes,
            out string outputMessage)
        {
            messageTypes = new List<Type>();
            outputMessage = "Handler instantiated.";

            if (implementationHandlerType.IsGenericType)
            {
                outputMessage = $"{implementationHandlerType.Name} can not be a Generic definition";
                return false;
            }

            var expectedType = sapherHandlerType;
            var implementedInterfaces = implementationHandlerType
                .GetInterfaces()
                .Where(i =>
                    i.IsGenericType
                    && i.GetGenericTypeDefinition() == expectedType)
                .ToList();

            if (implementedInterfaces.Count == 0)
            {
                outputMessage =
                    $"Expected implementation of generic {expectedType.Name}, " +
                    $"but received {implementationHandlerType.Name}";
                return false;
            }

            if (!allowMultipleHandlers
                && implementedInterfaces.Count > 1)
            {
                outputMessage =
                    $"{implementationHandlerType.Name} can only " +
                    $"implement {expectedType.Name} once";
                return false;
            }

            foreach (var implementedInterface in implementedInterfaces)
            {
                messageTypes.Add(implementedInterface.GenericTypeArguments[0]);
                serviceCollection.AddTransient(implementedInterface, implementationHandlerType);
            }

            return true;
        }
    }
}