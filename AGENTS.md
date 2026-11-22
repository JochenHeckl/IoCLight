# IoCLight

Unity package `de.jochenheckl.unity.ioclight` 3.2.0 — minimal IoC container built for il2cpp. Runtime namespace is `JH.IoCLight` (the 2.0.0 changelog entry calls it `IoCLight`). Trust `package.json` and source.

## Structure
- `Runtime/` — `JH.IoCLight` asmdef (no references). Entry points: `BootstrapBase`, `Container`/`IContainer`, `IoCBehaviour`, `[Inject]`, `MonoBehaviourIoCExtensions`.
- `Editor/` — `JH.IoCLight.Editor`, Editor-only; currently just the asmdef, no editor scripts.
- `Tests/` — `JH.IoCLight.Tests`, Editor-only, `UNITY_INCLUDE_TESTS`, `nunit.framework.dll`; source files use namespace `JH.IoCLight.Test` (singular).
- `Samples~/` — three importable samples; the `~` keeps them out of Unity's import until imported. They use `protected override void Compose()` and reference the runtime assembly by name in their asmdef.
- `.meta` files are committed alongside every asset; don't hand-edit them.

## Behaviour not inferable from names
- `BootstrapBase` creates the container in `Awake()`, calls `protected Compose()` there, then calls virtual `Shutdown()` from `OnDestroy()` and always terminates the container afterwards, even if `Shutdown()` throws — overrides cannot opt out of termination.
- `Container` registers itself as `IContainer` in its constructor.
- Bindings: `Register<T>()`, `RegisterInstance(x)`, `RegisterFactory(c => ...)`, then fluent `.As<I>()` / `.SingleInstance()`. `.As<I>()` changes only the lookup type and throws if `I` is not assignable from the resolved type.
- `Resolve<T>()` returns the FIRST registered match; `ResolveAll<T>()` returns every match — registration order is significant.
- Constructor injection picks the constructor with the most parameters and recursively resolves each parameter; if a parameter cannot be resolved because its type is not registered (`ResolutionFailedException`), it tries the next constructor and records the failure for the final error. Exceptions thrown by factories or other user code propagate and are never fallback candidates. A circular dependency throws `CircularDependencyException` and is never treated as a fallback candidate. `SingleInstance` caches per binding; `Container.Terminate()` terminates bindings in reverse registration order before clearing — custom bindings must implement it.
- `[Inject]` is inert unless something calls `IoCBehaviour.ResolveInjectables()` — nothing does it automatically. `IoCBehaviour.Resolve<T>()` and `FindContainer()` are lazy; `FindContainer()` requires a `BootstrapBase` on the same GameObject or an ancestor and throws otherwise — there is deliberately no scene-wide fallback. `IoCBehaviour` does not cache the container, so re-parenting or replacing the bootstrap is picked up on the next resolve. Injectable members are discovered by reflection once per behaviour type, walking the base classes below `MonoBehaviour`; `[Inject]` properties must be writable and fields must not be readonly/const.

## Verification
- CI is the self-contained source of truth: `.github/workflows/main.yml` runs `game-ci/unity-test-runner@v4.4.0` with `cliVersion: v0.1.76` and `packageMode: true` on Unity 6000.6.7f1 / 6000.0.82f1 / 2023.2.22f1 (ubuntu-latest, `UNITY_*` secrets).
- No Unity project lives in this repo. This folder is also the embedded `Packages/IoCLight` of the parent Autobattle sandbox; run focused tests from a host project, e.g. `unity status` then `unity command test run --mode editor --filter ContainerTest --project-path <host>` (see the parent `AGENTS.md`).
- EditMode tests build a bootstrap with `AddComponent<TestBootstrap>().Awake()` explicitly — do the same for new container tests.

## Style
- camelCase private fields, no `_` prefix. Indentation is 2 spaces, per the repo-root `.editorconfig`; format `.cs` files with csharpier.
