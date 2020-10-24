# Changelog

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).



## [1.4.0] - 2020-10-24
### Changed

- Container Property of BootstrapBase from
    ```csharp
    public Container Container { get; private set; }
    ```
    to
    ```csharp
    public IContainer Container { get; private set; }
    ```

- The material of the ColorSwitcher example was upgraded to URP.

### Added
- Added missing .meta files.


## [1.3.4] - 2020-10-17
### Added
- Added a check to type registration.
\
Registering a type **T** `.As<I>()` an other type **I**, which is not assignable from the original type **T** will now throw an exception.
\
This should help pointing out the problems with forgotten interface declarations etc.

## [1.3.3] - 2020-05-29
Changed root namespace and package name to de.JochenHeckl.Unity to unify with other packages

## [1.2.3] - 2020-05-22
Improved resolving types. Now an exception with a hint text is thrown if a type could not be resolved.

## [1.2.2] - 2020-05-06
Fixed namespace errors introduced in 1.2.1

## [1.2.1] - 2020-01-31
Changed namespace to de.JochenHeckl.IoCLight

## [1.2.0] - 2019-12-07
Added Bootstrap MainLoop Example

## [1.1.0] - 2019-12-07
Implemented support for RegisterInstance and SingleInstance as well constructor parameter resolution

## [1.0.0] - 2019-12-05
Initial Release