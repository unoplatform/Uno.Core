# Uno.Core

Uno.Core is nventive's attempt at filling some gaps in the existing .NET Base Class Library and related technologies. It reduces friction
and increases the predictability of the API. It consists of a set of helpers, extension methods and additional abstractions.

The following packages are available:
- `Uno.Core.Extensions` is a set of extension methods on common .NET times such as `Func<>`, `Action<>`, `Regex`, `Stream`, etc...
- `Uno.Core.Extensions.Collections` is a set of collection-related extensions and types such as `ICollection`, `IEnumerable`, `Memory`, etc...
- `Uno.Core.Extensions.Disposables` is a set of `IDisposable` implementations
- `Uno.Core.Extensions.Equality` is a set of specialized equality comparers
- `Uno.Core.Extensions.Logging` is a set of extension methods for `Microsoft.Logging.Extensions` provided types
- `Uno.Core.Extensions.Logging.Singleton` is a helper to get a singleton-based `this.Log()` logging.
- `Uno.Core.Extensions.Threading` is a set of helpers for threading such as `Uno.Transactional` or `Uno.Threading.FastAsyncLock`.
- `Uno.Core.Extensions.Compatibility` is present for compatibility with existing applications
- `Uno.Core` is a meta-package which references all above packages.

## Build status

| Target | Branch | Status | Recommended Nuget packages version |
| ------ | ------ | ------ | ------ |
| development | master |[![Build Status](https://dev.azure.com/uno-platform/Uno%20Platform/_apis/build/status/Uno%20Platform/Uno.Core-CI?branchName=master)](https://dev.azure.com/uno-platform/Uno%20Platform/_build/latest?definitionId=39?branchName=master) | [![NuGet](https://img.shields.io/nuget/v/Uno.Core.svg)](https://www.nuget.org/packages/Uno.Core/) |

# Have questions? Feature requests? Issues?

Make sure to visit our [StackOverflow](https://stackoverflow.com/questions/tagged/uno-platform), [create an issue](https://github.com/nventive/Uno.SourceGeneration/issues) or [visit our gitter](https://gitter.im/uno-platform/Lobby).