# Changelog

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [3.1.0] - 2026-11-21

### Added
- `CircularDependencyException` for dependency cycles. A cycle is a hard failure and is never treated as a failed constructor candidate.
- Failed constructor candidates contribute their messages to the final resolution error.
- Injectable members are discovered once per behaviour type and validated: `[Inject]` properties must be writable, fields must not be readonly or const.
- `FindContainer()` reports a distinct error when a bootstrap exists but its `Awake()` has not run.

### Changed
- `RegisterInstance(null)` and `RegisterFactory(null)` throw `ArgumentNullException`.
- Constructor injection tries the next constructor when a dependency cannot be resolved, instead of failing immediately.
- `Container.Terminate()` terminates bindings in reverse registration order and clears the resolution stack.
- `BootstrapBase.OnDestroy()` does nothing without a container and always terminates it, even if `Shutdown()` throws.
- `FindContainer()` requires a `BootstrapBase` on the same GameObject or an ancestor; the scene-wide fallback was removed.
- `IoCBehaviour` no longer caches the container, so re-parenting or replacing a bootstrap is picked up on the next resolve.
- Factory bindings call their producer directly with the container instead of invoking it through reflection.

### Removed
- Unused `InstanceBinding<T>` and the empty `BootstrapBaseEditor` custom editor.

## [3.0.0] - 2026-11-21

### Changed
- BREAKING: package id changed from `jh.ioclight` to `de.jochenheckl.unity.ioclight`.
- BREAKING: `BootstrapBase.Compose()` is now `protected` instead of `public`.
- BREAKING: `ITypeBinding` gained a `Terminate()` method that custom bindings must implement.
- `BootstrapBase` gained a `protected virtual Shutdown()` teardown hook. It runs before container termination, which remains unconditional in `OnDestroy()`.
- The `Bootstrap MainLoop` sample is now `Bootstrap Main Loop`, and its `RuntimeData` tracks `SystemOneData` and `SystemTwoData` instead of simulation time.

### Added
- `Container.Terminate()` clears cached singleton instances from every binding before clearing the binding list.
- The `Bootstrap Main Loop` sample registers `SystemOne` and `SystemTwo` as `ISystem` implementations driven by `Start()`, `FixedUpdate()` and `Shutdown()`.

## [2.0.0] - 2024-02-10
- Main namespace was shortened to IoCLight
- Register API changed to return ITypeBinding instead of TypeBindingBase
- Adds new way of registering: ```public ITypeBinding RegisterFactory<ProductType>( Func<IContainer, ProductType> producer )```
- Adds IoCBehaviour which enables resolving instances from within MonoBehaviour
- Other Minor Improvements

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

### Fixed

- Fixed broken namespaces in the bootstrap source files.

## [1.3.4] - 2020-10-17

### Added

- Added a check to type registration.
  Registering a type **T** `.As<I>()` an other type **I**, which is not assignable from the original type **T** will now throw an exception.

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
