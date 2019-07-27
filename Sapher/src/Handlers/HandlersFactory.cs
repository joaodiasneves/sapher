namespace Sapher.Handlers
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;

    public static class HandlersFactory
    {
        public static bool TryToGenerateHandlerInfo<T>(
            Type handlerType,
            IServiceCollection serviceCollection,
            out T handlerInstance,
            out Type messageType,
            out string outputMessage)
        {
            handlerInstance = default(T);
            messageType = default(Type);
            outputMessage = "Handler instantiated.";

            var expectedType = typeof(T);

            if (handlerType == expectedType
                || !handlerType
                    .GetInterfaces()
                    .Any(i => i.IsGenericType && i.IsAssignableFrom(expectedType)))
            {
                outputMessage =
                    $"Expected implementation of generic {expectedType.Name}, " +
                    $"but received {handlerType.Name}";
                return false;
            }

            if (handlerType.IsGenericType)
            {
                outputMessage = $"{handlerType.Name} can not be a Generic definition";
                return false;
            }

            handlerInstance = (T)serviceCollection
                .BuildServiceProvider()
                .GetRequiredService(handlerType);

            messageType = handlerType
                .GetTypeInfo()
                .GenericTypeArguments[0];

            return true;
        }
    }
}