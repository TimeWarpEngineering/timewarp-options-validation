# Fix AOT and Trim Warnings

## Summary

Package produces IL2104/IL3053 trim and AOT analysis warnings when used in applications published with `PublishAot=true`. The package needs to be made fully AOT-compatible.

**GitHub Issue:** #7

## Todo List

- [x] Identify reflection usage causing trim warnings
- [x] Add `[DynamicallyAccessedMembers]` attributes where needed
- [x] Consider source generators for reflection-based binding
- [x] Add `<IsAotCompatible>true</IsAotCompatible>` to project
- [x] Add `<IsTrimmable>true</IsTrimmable>` to project
- [x] Test with AOT publish to verify zero warnings
- [ ] Update package version

## Notes

**Current warnings:**
```
TimeWarp.OptionsValidation.dll : warning IL2104: Assembly 'TimeWarp.OptionsValidation' produced trim warnings.
TimeWarp.OptionsValidation.dll : warning IL3053: Assembly 'TimeWarp.OptionsValidation' produced AOT analysis warnings.
```

**Reproduction steps:**
1. Create a .NET 10 application using `TimeWarp.OptionsValidation`
2. Use `AddFluentValidatedOptions<TOptions, TValidator>(config)`
3. Publish with AOT: `dotnet publish -p:PublishAot=true`

**Package Version:** 1.0.0-beta.3

**Related:** TimeWarp.Nuru task 018-make-configuration-validation-sample-aot-compatible

## Results

### Changes Made:

**1. `service-collection-extensions.cs`**
- Added `using System.Diagnostics.CodeAnalysis;`
- Added `[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]` to `TOptions` on the configuration binding overload (required by `Bind<TOptions>()`)
- Added `[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]` to `TValidator` on both overloads

**2. `options-builder-extensions.cs`**
- Added `using System.Diagnostics.CodeAnalysis;`
- Added `[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]` to `TValidator` (required by `TryAddSingleton<TValidator>()`)

**3. `timewarp-options-validation.csproj`**
- Added `<IsAotCompatible>true</IsAotCompatible>`
- Added `<IsTrimmable>true</IsTrimmable>`

**4. New AOT test project** (`tests/aot-test/`)
- Created test project that publishes with AOT to verify no trim/AOT warnings

### Test Results:

| Test | Result |
|------|--------|
| `dotnet build -warnaserror` | ✅ 0 warnings, 0 errors |
| `dotnet test` | ✅ Passed: 4, Skipped: 1 |
| `dotnet publish -c Release` (AOT) | ✅ 0 IL2104/IL3053 warnings |
| Run AOT binary | ✅ Output: "Name: Test, Value: 42" |

### Decision:
Source generators were not needed - the `[DynamicallyAccessedMembers]` attributes were sufficient to eliminate all warnings while maintaining full AOT compatibility.
