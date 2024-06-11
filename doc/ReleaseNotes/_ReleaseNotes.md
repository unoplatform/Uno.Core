# Release notes

## Next version

* `Uno.Extensions.ListExtensions.AsReadOnly` is now removed on net7.0 and later as it will be ambiguous with the BCL one (`System.Collections.Generic.CollectionExtensions.AsReadOnly`). Use the method from BCL.

* `Uno.Extensions.EnumerableExtensions.SkipLast` is now removed on net7.0 and later and netstandard2.1 as it will be ambiguous with the BCL one (`System.Linq.Enumerable.SkipLast`). Use the method from BCL.


### Features
* 
### Breaking changes
* 
### Bug fixes
* 

## Release 1.28.0

### Features
* Add Span<T> and Memory<T> extensions

## Release 1.27.0

### Features

 * Direct compatibility with projects targeting uap 10.0.17763
 * Adds a log extension point with a provided log level and message builder.

### Breaking changes

 * Removed CategoryAttribute and ObfuscationAttribute from the lib. If your project uses them, change the target SDK to UAP 10.0.17763 or higher.


## Release 1.26.0

* No major changes.
