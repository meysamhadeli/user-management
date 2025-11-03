# 👤 User Management

> 🚀 **A full-stack application with the latest technologies and architecture like Vertical Slice Architecture, CQRS, Aspire, .Net 9 amd Angular 20.**

<div>
  <a href='https://codespaces.new/meysamhadeli/user-management?quickstart=1'><img alt='Open in GitHub Codespaces' src='https://github.com/codespaces/badge.svg'></a>
</div>

# Table of Contents

- [The Goals of This Project](#the-goals-of-this-project)
- [The Domain and Bounded Context](#the-domain-and-bounded-context)
- [Structure of Project](#structure-of-project)
  - [Backend Structure](#backend-structure)
  - [Frontend Structure](#frontend-structure)
- [Development Setup](#development-setup)
    - [Dotnet Tools Packages](#dotnet-tools-packages)
    - [Husky](#husky)
    - [Upgrade Nuget Packages](#upgrade-nuget-packages)
- [How to Run](#how-to-run)
  - [Aspire](#aspire)
  - [Build](#build)
  - [Run](#run)
  - [Test](#test)
- [Documentation Apis](#documentation-apis)
- [License](#license)

## The Goals of This Project

### Backend
- :sparkle: Using `Vertical Slice Architecture` for `architecture` level.
- :sparkle: Using `CQRS` implementation with `MediatR` library.
- :sparkle: Using `Postgres` for `write side` database.
- :sparkle: Using `Unit Testing` for testing small units and mocking our dependencies with `Nsubstitute`.
- :sparkle: Using `End-To-End Testing` and `Integration Testing` for testing `features` with all dependencies using `testcontainers`.
- :sparkle: Using `Fluent Validation` and a `Validation Pipeline Behaviour` on top of `MediatR`.
- :sparkle: Using `Minimal API` for all endpoints.
- :sparkle: Using `AspNetCore OpenApi` for `generating` built-in support `OpenAPI documentation` in ASP.NET Core.
- :sparkle: Using  `OpenTelemetry` for collecting `Logs`, `Metrics` and `Distributed Traces`.
- :sparkle: Using `Aspire` for `service discovery`, `observability`, and `local orchestration` of backend and frontend.

### Frontend
- :sparkle: Using `Angular 20` with modern features and best practices.
- :sparkle: Using `Vertical Slice Architecture` and `Feature Folder Structure` for `architecture` level.
- :sparkle: Using `RxJS` for reactive programming and state management.
- :sparkle: Using `Standalone Components` and modern Angular APIs.

## The Domain And Bounded Context

- `UserManagement`: A bounded context that manages the complete 4-step registration workflow for companies and users, ensuring data validation, business rules enforcement.

## Structure of Project

### Backend Structure

In the backend project, I used [vertical slice architecture](https://jimmybogard.com/vertical-slice-architecture/) at the architectural level and [feature folder structure](http://www.kamilgrzybek.com/design/feature-folders/) to structure my files.

I treat each request as a distinct use case or slice, encapsulating and grouping all concerns from front-end to back.
When adding or changing a feature in an application in n-tire architecture, we are typically touching many "layers" in an application. We are changing the user interface, adding fields to models, modifying validation, and so on. Instead of coupling across a layer, we couple vertically along a slice. We `minimize coupling` `between slices`, and `maximize coupling` `in a slice`.

With this approach, each of our vertical slices can decide for itself how to best fulfill the request. New features only add code, we're not changing shared code and worrying about side effects.

<div align="center">
  <img src="./assets/vertical-slice-architecture.png" />
</div>

Instead of grouping related action methods in one controller, as found in traditional ASP.net controllers, I used the [REPR pattern](https://deviq.com/design-patterns/repr-design-pattern). Each action gets its own small endpoint, consisting of a route, the action, and an `IMediator` instance (see [MediatR](https://github.com/jbogard/MediatR)). The request is passed to the `IMediator` instance, routed through a [`Mediatr pipeline`](https://lostechies.com/jimmybogard/2014/09/09/tackling-cross-cutting-concerns-with-a-mediator-pipeline/) where custom [middleware](https://github.com/jbogard/MediatR/wiki/Behaviors) can log, validate and intercept requests. The request is then handled by a request specific `IRequestHandler` which performs business logic before returning the result.

The use of the [mediator pattern](https://dotnetcoretutorials.com/2019/04/30/the-mediator-pattern-in-net-core-part-1-whats-a-mediator/) in my controllers creates clean and [thin controllers](https://codeopinion.com/thin-controllers-cqrs-mediatr/). By separating action logic into individual handlers we support the [Single Responsibility Principle](https://en.wikipedia.org/wiki/Single_responsibility_principle) and [Don't Repeat Yourself principles](https://en.wikipedia.org/wiki/Don%27t_repeat_yourself), this is because traditional controllers tend to become bloated with large action methods and several injected `Services` only being used by a few methods.

I used CQRS to decompose my features into small parts that makes our application:

- Maximize performance, scalability and simplicity.
- Easy to maintain and add features to. Changes only affect one command or query, avoiding breaking changes or creating side effects.
- It gives us better separation of concerns and cross-cutting concern (with help of mediatr behavior pipelines), instead of bloated service classes doing many things.

Using the CQRS pattern, we cut each business functionality into vertical slices, for each of these slices we group classes (see [technical folders structure](http://www.kamilgrzybek.com/design/feature-folders)) specific to that feature together (command, handlers, infrastructure, repository, controllers, etc). In our CQRS pattern each command/query handler is a separate slice. This is where you can reduce coupling between layers. Each handler can be a separated code unit, even copy/pasted. Thanks to that, we can tune down the specific method to not follow general conventions (e.g. use custom SQL query or even different storage). In a traditional layered architecture, when we change the core generic mechanism in one layer, it can impact all methods.

### Frontend Structure

For the frontend, we use **Angular 20** and follow the same architectural principles as the backend:

- **Vertical Slice Architecture** - Each feature is organized as a self-contained vertical slice
- **Feature Folder Structure** - All components, services, models, and logic related to a feature are grouped together

The frontend application is structured around business features rather than technical layers. Each feature contains:
- Feature-specific components
- Services and state management
- Models and interfaces
- Routing configuration
- Tests specific to the feature

This approach ensures:
- High cohesion within features
- Minimal coupling between features
- Easy maintenance and feature development
- Consistent architecture across the full stack
- Independent development and testing of features

We leverage modern Angular features including:
- Standalone components and APIs
- Reactive programming with RxJS
- Type-safe development practices

## Development Setup

### Dotnet Tools Packages
For installing our requirement packages with .NET cli tools, we need to install `dotnet tool manifest`.
```bash
dotnet new tool-manifest
```
And after that we can restore our dotnet tools packages with .NET cli tools from `.config` folder and `dotnet-tools.json` file.
```
dotnet tool restore
```

### Husky
Here we use `husky` to handel some pre commit rules and we used `conventional commits` rules and `formatting` as pre commit rules, here in [package.json](./package.json). of course, we can add more rules for pre commit in future. (find more about husky in the [documentation](https://typicode.github.io/husky/get-started.html))
We need to install `husky` package for `manage` `pre commits hooks` and also I add two packages `@commitlint/cli` and `@commitlint/config-conventional` for handling conventional commits rules in [package.json](.././package.json).
Run the command bellow in the root of project to install all npm dependencies related to husky:

```bash
npm install
```

> Note: In the root of project we have `.husky` folder and it has `commit-msg` file for handling conventional commits rules with provide user friendly message and `pre-commit` file that we can run our `scripts` as a `pre-commit` hooks. that here we call `format` script from [package.json](./package.json) for formatting purpose.

### Upgrade Nuget Packages
For upgrading our nuget packages to last version, we use the great package [dotnet-outdated](https://github.com/dotnet-outdated/dotnet-outdated).
Run the command below in the root of project to upgrade all of packages to last version:
```bash
dotnet outdated -u
```

## How to Run

> ### Aspire

To run the application using the `Aspire`, execute the following command from the solution root:

```bash
aspire run
```

> Note:The `Aspire dashboard` will be available at `http://localhost:18888`

> ### Build
To `build` app, run this command in the `root` of the project:
```bash
dotnet build
```

> ### Run
To `run` app, run this command in the `root` of the project:
```bash
dotnet run
```

> ### Test

To `test` app, run this command in the `root` of the project:
```bash
dotnet test
```

> ### Documentation Apis

For checking `API documentation`, navigate to `/swagger` for `Swagger OpenAPI` or `/scalar/v1` for `Scalar OpenAPI` to visit list of endpoints.

## License
This project is made available under the MIT license. See [LICENSE](https://github.com/meysamhadeli/user-management/blob/main/LICENSE) for details.