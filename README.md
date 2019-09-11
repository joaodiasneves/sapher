# Sapher - *Sa*ga Choreogra*pher*
[![NuGet version](https://badge.fury.io/nu/sapher.svg)](https://badge.fury.io/nu/sapher)

Sapher seeks to help developers implement communication between services in a microservices context, and was built with **choreographed sagas** in mind - more context [here](https://github.com/joaodiasneves/sapher/wiki/01.-Context).

Usually, in a microservices architecture:

1. A business process spans across multiple services.
2. Each service executes its own step of the process. 
3. Each step naturally has a single input, and can expect multiple responses depending on the input execution.
4. It may or may not execute compensating operations when receiving the responses.

Sapher was modelled around this 4 main ideas, represented in the following model:
![domain_model_sapher.png](https://drive.google.com/file/d/18w9eaUIxp0Hbj3BGyhjXphoToLEE2H25/view?usp=sharing)

## Table of Contents
* [Getting Started](#Getting-Started)
    - [Main Concepts](#Main-Concepts)
    - [Setup & Execution](#Setup-&-Execution)
        - [Input Handler](#Input-Handler)
        - [Response Handler](#Response-Handler)
        - [Configuration](#Configuration)
        - [Message Delivery](#Message-Delivery)
        - [Retrieving Step state](#Retrieving-Step-state)
    - [Prerequisites](#Prerequisites)
    - [Installing via NuGet](#Installing-via-NuGet)
* [Built With](#Built-With)
* [Extensibility](#Extensibility)
* [Contributing](#Contributing)
* [Versioning](#Versioning)
* [Authors](#Authors)
* [License](#License)
* [Acknowledgments](#Acknowledgments)

## Getting Started

These instructions will help you start using Sapher, saphely (completely intended pun, sorry).

### Main Concepts

1. **InputHandler:** Implementation logic for handling a message that is defined as the input of a step.
2. **ResponseHandler:** Implementation logic for handling a message that is defined as the response of a step.
3. **MessageSlip:** Object containing information of a specific message, in order to correlate it with other messages:
3.1. *MessageId*: a unique identifier of the message.
3.2. *CorrelationId*: a unique identifier of the business process (aka Distributed Transaction) the messages belong to.
3.3. *ConversationId*: the MessageId of the message that originated this one.

### Setup & Execution

Sapher allows the definition of clear interfaces for step input handling, and response handling, while also providing the possibility to reuse message handlers on different steps, helping to maintain clean code.
The first step of Sapher setup should be to define those interfaces.

#### Input Handler
```
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
**InputResult.SentMessageIds** is crucial to correlate the input execution with the following responses. If left empty or null, the step is considered successful. If filled with Ids, Sapher will mark the Step as Executed. Only when all the responses have been received, Sapher will mark the step as Successful.
**InputResult.DataToPersist** is used to persist information that may be used for ResponseHandlers (for instance, for compensating operations).

#### Response Handler
```
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
In this example, a compensating operation is executed. Also, **previouslyPersistedData** cnotains the data persisted by the previous executions, which is useful for the compensating operation. 
The response handler can modify this data, and following response handlers will receive the modifications.
**Note:** ISapherDataRepository implementation is responsible for handling concurrency.

#### Configuration
To wire things up, lets assume the handlers described above belong to the same step. Their configuration would be as follows:
```
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

#### Message Delivery
After implementing all the required handlers and setting up Sapher configuration, it is now time for some action. When receiving a message, either via HTTP, a message broker, or any other way, you can use `ISapher` to deliver it to your handlers. Using `Microsoft.Extensions.DependencyInjection`, `ISapher` will be injected in your code. 
Therefore, all you have to do, is to execute the following steps:
```
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

#### Retrieving Step state
To obtain persisted information regarding a SapherStep, you can do the following:
```
// option 1
var dataRetrieved = await this.sapher.GetStepInstance(stepName, inputMessageId);

// or 

// option 2
var listOfDataRetrieved = await this.sapher.GetStepInstances(stepName);
```

- *Option 1* will identify a specific instance of a Step, using the step name and the unique id of the input message that created the instance.
- *Option 2* will identify all the instances of a step with the given step name.


### Prerequisites
* Supports .NET Standard
* Requires the usage of ServiceCollection for dependency injection.


### Installing via NuGet
Sapher is a library available in [NuGet](https://www.nuget.org/packages/sapher), and it should be installed in any software where your business process has steps  running.

```
Install-Package Sapher
```

## Built With
* [Polly](https://github.com/App-vNext/Polly) - Used to implement Retry policies.

## Extensibility
**Persistence** engine can be extended by implementing `ISapherDataRepository`
```
MyRepositoryImplementation : ISapherDataRepository
```
and providing the implementation with `AddPersistence`.
```
this.serviceCollection
    .AddSapher(sapherConfig => sapherConfig
        .AddPersistence<MyRepositoryImplementation>());

// or

var myRepository = new MyRepositoryImplementation();
this.serviceCollection
    .AddSapher(sapherConfig => sapherConfig
        .AddPersistence(myRepository));
```

**Notes:** `ISapherDataRepository` is used as a singleton. Also, if an implementation is not defined, Sapher will use in-memory persistence.

**Logging** engine can also be extended by implementing `ILogger`
```
MyLogger : ILogger
```
and providing the implementation with `AddLogger`
```
this.serviceCollection
    .AddSapher(sapherConfig => sapherConfig
        .AddLogger<MyLogger>());
        
// or 

var myLogger = new MyLogger();
this.serviceCollection
    .AddSapher(sapherConfig => sapherConfig
        .AddLogger(myLogger));
```
**Note:** `ILogger` is used as a singleton. Also, if an implementation is  not defined, Sapher will not log anything.

## Contributing
Please read [CONTRIBUTING.md](https://github.com/joaodiasneves/sapher/blob/master/CONTRIBUTING.md) for details on the code of conduct, and the process for submitting pull requests to this project.

## Versioning
This project uses [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/joaodiasneves/sapher/tags). 

## Authors
* **João Neves** - *Initial work* - [joaodiasneves](https://github.com/joaodiasneves)

See also the list of [contributors](https://github.com/joaodiasneves/sapher/contributors) who participated in this project.

## License
This project is licensed under the Apache 2.0 license - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments
For being inspiration and references for this project:
* Adam Ralph
* Daniel Gerlag
* James Lewis
* Martin Fowler
* Sam Newman

For having the patience (and the passion) to discuss with me some topics regarding software engineering and architecture:
* André Correia
* Isabel Azevedo
* Jorge Loureiro
* Luis Pinto
