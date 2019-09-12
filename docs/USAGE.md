# Getting Started

These instructions help you start using Sapher, saphely (completely intended pun, sorry).

## Prerequisites
* Supports .NET Standard
* Requires the usage of ServiceCollection for dependency injection.

## Installing via NuGet
Sapher is a library available in [NuGet](https://www.nuget.org/packages/sapher), and it should be installed in any software where your business process has steps running.

```
Install-Package Sapher
```

## Main Concepts

1. **InputHandler:** Implementation logic for handling a message that is defined as the input of a step.
2. **ResponseHandler:** Implementation logic for handling a message that is defined as the response of a step.
3. **MessageSlip:** Object containing information of a specific message, to correlate it with other messages:
3.1. *MessageId*: a unique identifier of the message.
3.2. *CorrelationId*: a unique identifier of the business process (aka Distributed Transaction) the messages belong to.
3.3. *ConversationId*: the MessageId of the message that originated this one.

## Setup & Execution

Sapher allows the definition of clear interfaces for step input handling, and response handling, while also providing the possibility to reuse message handlers on different steps, helping to maintain clean code.
The first step of Sapher setup should be to define those interfaces.

### Input Handler
```c#
using Sapher.Handlers

public class ExampleInputHandler : IHandlesInput<InputMessage>
{
    public async Task<InputResult> Execute(InputMessage message, MessageSlip messageSlip)
    {
        Console.WriteLine("Executing InputMessage");
        var publishedMessageSlip = await this.exampleService.DoStuff();
        
        if(publishedMessageSlip != null)
        {
            return new InputResult
            {
                SentMessageIds = new List<string> { publishedMessageSlip.MessageId },
                DataToPersist = new Dictionary<string, string>
                {
                    { "AffectedEntityId", message.EntityId.ToString() }
                },
                State = InputResultState.Successful
            };    
        }
        
        return new InputResult
        {
            State = InputResultState.Failed
        }; 
    }
}
```
**InputResult.SentMessageIds** is crucial to correlate the input execution with the following responses. If left empty or null, the step is considered successful. If filled with Ids, Sapher marks the Step as Executed. Only when all the responses are received, Sapher marks the step as Successful.
**InputResult.DataToPersist** is used to persist information that is by ResponseHandlers (for instance, for compensating operations).

### Response Handler
```c#
using Sapher.Handlers

public class ExampleResponseHandler : IHandlesResponse<ResponseMessage>
{
    public Task<ResponseResult> Execute(
            ResponseMessage message,
            MessageSlip messageSlip,
            IDictionary<string, string> previouslyPersistedData)
    {
        Console.WriteLine("Executing ResponseMessage");
        var success = await exampleService.DeleteEntity(previouslyPersistedData["AffectedEntityId"])
        
        if(success)
        {
            previouslyPersistedData.Add("stuffHeader","stuffValue");

            return new ResponseResult
            {
                State = ResponseResultState.Compensated,
                DataToPersist = previouslyPersistedData
            };    
        }
        
        return new ResponseResult
        {
                State = ResponseResultState.Failed,
                DataToPersist = previouslyPersistedData
        }; 

        return Task.FromResult(result);
    }
}
```
In this example, a compensating operation is executed. Also, **previouslyPersistedData** contains the data persisted by the previous executions, which is useful for the compensating operation. 
The response handler can modify this data, and following response handlers will receive the modifications.

**Note:** `ISapherDataRepository` implementation is responsible for handling concurrency.

### Configuration
To wire things up, lets assume the handlers described above belong to the same step. Their configuration would be as follows:
```c#
this.serviceCollection.AddSapher(sapherConfig => sapherConfig
    .AddStep<ExampleInputHandler>("ExampleStepName", stepConfig => stepConfig
        .AddResponseHandler<ExampleResponseHandler>()
        .AddResponseHandler<AnotherExampleResponseHandler>())
    .AddStep<ExampleStepWithOnlyInput>("ExampleStepOnlyInput")
    .AddStep<ExampleStepWithOtherInputHandler>("ReusingResponseHandlers", , stepConfig => stepConfig
        .AddResponseHandler<ExampleResponseHandler>())
    .AddRetryPolicy(settings.SapherMaxRetries, settings.SapherRetryIntervalMs)
    .AddTimeoutPolicy(settings.SapherTimeoutMs));
    
(...)

this.sapher = this.serviceProvider.UseSapher();
```
In the example above, three steps were defined. The first, has an input and two types of responses. The second, does not expect any response. The third, shows an example of reusing a response handler in different steps.

**Note:** **Retry** and **Timeout** policies configuration was also exemplified.

### Message Delivery
After implementing all the required handlers and setting up Sapher configuration, it is now time for some action. When receiving a message, either via HTTP, a message broker, or any other way, you can use `ISapher` to deliver it to your handlers. Using `Microsoft.Extensions.DependencyInjection`, `ISapher` will be injected in your code. 
Therefore, all you have to do, is to execute the following steps:
```c#
var deliveryResult = await this.sapher
    .DeliverMessage(
        new InputMessage
        {
            Value = someValue,
            EntityId = someId
        },
        Sapher.Dtos.MessageSlip.GenerateNewMessageSlip());
```
Sapher will deliver the message to all the configured SapherSteps which have an handler for it (either Input or Response handler).
The result of `DeliverMessage` is `DeliveryResult`. When successful, DeliveryResult will contain `StepResults` with their respective `InputResult` or `ResponseResult`.
If not successful, `DeliveryResult` provides information regarding the error occurred (e.g. Error message, Exception thrown...).

**Notes:** 
- If the message matches a response handler, but no step instance is identified, the message will be ignored. **To execute responses, an input message must have been executed before so that the step is instantiated.**. 
- Sapher maps responses to step instances using the response message conversationId and comparing it with the input execution sent messageid. Therefore, using MessageSlip concepts is mandatory. `MessageSlip` class provides some methods to help with this.

### Retrieving Step state
To obtain persisted information regarding a SapherStep, you can do the following:
```c#
// option 1
var dataRetrieved = await this.sapher.GetStepInstance(stepName, inputMessageId);

// or 

// option 2
var listOfDataRetrieved = await this.sapher.GetStepInstances(stepName);
```

- *Option 1* will identify a specific instance of a Step, using the step name and the unique id of the input message that created the instance.
- *Option 2* will identify all the instances of a step with the given step name.
