# IoCLight

Unity package `de.jochenheckl.unity.ioclight` 3.0.0 — minimal IoC container built for il2cpp. Runtime namespace is `JH.IoCLight` (the 2.0.0 changelog entry calls it `IoCLight`). Trust `package.json` and source.

## Structure
- `Runtime/` — `JH.IoCLight` asmdef (no references). Entry points: `BootstrapBase`, `Container`/`IContainer`, `IoCBehaviour`, `[Inject]`, `MonoBehaviourIoCExtensions`.
- `Editor/` — `JH.IoCLight.Editor`, Editor-only; only an empty custom editor for `BootstrapBase`.
- `Tests/` — `JH.IoCLight.Tests`, Editor-only, `UNITY_INCLUDE_TESTS`, `nunit.framework.dll`; source files use namespace `JH.IoCLight.Test` (singular).
- `Samples~/` — three importable samples; the `~` keeps them out of Unity's import until imported. They use `protected override void Compose()` and reference the runtime assembly by name in their asmdef.
- `.meta` files are committed alongside every asset; don't hand-edit them.

## Behaviour not inferable from names
- `BootstrapBase` creates the container in `Awake()`, calls `protected Compose()` there, then calls virtual `Shutdown()` from `OnDestroy()` and always terminates the container afterwards — overrides cannot opt out of termination.
- `Container` registers itself as `IContainer` in its constructor.
- Bindings: `Register<T>()`, `RegisterInstance(x)`, `RegisterFactory(c => ...)`, then fluent `.As<I>()` / `.SingleInstance()`. `.As<I>()` changes only the lookup type and throws if `I` is not assignable from the resolved type.
- `Resolve<T>()` returns the FIRST registered match; `ResolveAll<T>()` returns every match — registration order is significant.
- Constructor injection picks the constructor with the most parameters and recursively resolves each parameter; if one parameter fails, it throws rather than falling back to a shorter constructor. `SingleInstance` caches per binding; `Container.Terminate()` calls `ITypeBinding.Terminate()` on every binding before clearing — custom bindings must implement it.
- `[Inject]` is inert unless something calls `IoCBehaviour.ResolveInjectables()` — nothing does it automatically. `IoCBehaviour.Resolve<T>()` and `FindContainer()` are lazy; `FindContainer()` walks up to a `BootstrapBase` then falls back to a scene-wide search behind `#if UNITY_6000_4_OR_NEWER`. Preserve that guard when editing it.

## Verification
- CI is the self-contained source of truth: `.github/workflows/main.yml` runs `game-ci/unity-test-runner@v4` with `packageMode: true` on Unity 6000.7.0f1 / 6000.4.0f1 / 6000.3.7f1 / 2023.2.22f1 (ubuntu-latest, `UNITY_*` secrets).
- No Unity project lives in this repo. This folder is also the embedded `Packages/IoCLight` of the parent Autobattle sandbox; run focused tests from a host project, e.g. `unity status` then `unity command test run --mode editor --filter ContainerTest --project-path <host>` (see the parent `AGENTS.md`).
- EditMode tests build a bootstrap with `AddComponent<TestBootstrap>().Awake()` explicitly — do the same for new container tests.

## Style
- camelCase private fields, no `_` prefix. Indentation is 4 spaces in most files, but 2 in `IoCBehaviour.cs`, `InjectAttribute.cs`, and `MonoBehaviourIoCExtensions.cs` — match the file.
