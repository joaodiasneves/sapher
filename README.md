# Saga Choreographer - Sapher Distributed Transactions

* Supports .NET Core
* Requires Usage of ServiceCollection.

## Provided features
* Idempotency
* Retry policy
* Distributed transaction management using a choreographic approach

## Extensibility
* Persistence engine can be extended by implementing ISapherDataRepository and providing the implementation with AddPersistence.
* Logging engine can be extended by implementing ILogger and providing the implementation with AddLogger

## Thid Party Libraries Used
* [Polly](https://github.com/App-vNext/Polly)
* [FluentAssertions](https://github.com/fluentassertions/fluentassertions)
