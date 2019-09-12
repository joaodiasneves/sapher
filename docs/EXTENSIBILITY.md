Persistence engine can be extended by implementing `ISapherDataRepository`
```c#
MyRepositoryImplementation  ISapherDataRepository
```
and providing the implementation with `AddPersistence`.
```c#
this.serviceCollection
    .AddSapher(sapherConfig = sapherConfig
        .AddPersistenceMyRepositoryImplementation());

 or

var myRepository = new MyRepositoryImplementation();
this.serviceCollection
    .AddSapher(sapherConfig = sapherConfig
        .AddPersistence(myRepository));
```

Notes `ISapherDataRepository` is used as a singleton. Also, if an implementation is not defined, Sapher will use in-memory persistence.

Logging engine can also be extended by implementing `ILogger`
```c#
MyLogger  ILogger
```
and providing the implementation with `AddLogger`
```c#
this.serviceCollection
    .AddSapher(sapherConfig = sapherConfig
        .AddLoggerMyLogger());
        
 or 

var myLogger = new MyLogger();
this.serviceCollection
    .AddSapher(sapherConfig = sapherConfig
        .AddLogger(myLogger));
```
Note `ILogger` is used as a singleton. Also, if an implementation is  not defined, Sapher will not log anything.