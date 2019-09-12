# Sapher - *Sa*ga Choreogra*pher*
[![NuGet version](https://badge.fury.io/nu/sapher.svg)](https://badge.fury.io/nu/sapher) [![Build Status](https://travis-ci.com/joaodiasneves/sapher.svg?branch=master)](https://travis-ci.com/joaodiasneves/sapher)

Sapher seeks to help developers implement communication between services in a microservices context, and was built with **choreographed sagas** in mind - more context [here](https://github.com/joaodiasneves/sapher/wiki/01.-Context).

Usually, in a microservices architecture:

1. A business process spans across multiple services.
2. Each service executes its step of the process. 
3. Each step naturally has a single input and can expect multiple responses depending on the input execution.
4. It may or may not execute compensating operations when receiving the responses.

Sapher is modelled around these 4 main ideas, which are detailed and represented [here](https://github.com/joaodiasneves/sapher/wiki/02.-Domain-model)

## Getting Started

Please read [the guide](https://github.com/joaodiasneves/sapher/blob/master/docs/USAGE.md) to start using Sapher.

## Documentation

More details are provided in [wiki](https://github.com/joaodiasneves/sapher/wiki) pages.

## Built With
* [Polly](https://github.com/App-vNext/Polly) - Used to implement Retry policies.

## Extensibility

Sapher provides extension points for Persistence and Logging. 
Please read [EXTENSIBILITY.md](https://github.com/joaodiasneves/sapher/blob/master/docs/EXTENSIBILITY.md) to understand how you can do this.

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
