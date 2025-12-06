# Fix AOT and Trim Warnings

## Summary

Package produces IL2104/IL3053 trim and AOT analysis warnings when used in applications published with `PublishAot=true`. The package needs to be made fully AOT-compatible.

**GitHub Issue:** #7

## Todo List

- [ ] Identify reflection usage causing trim warnings
- [ ] Add `[DynamicallyAccessedMembers]` attributes where needed
- [ ] Consider source generators for reflection-based binding
- [ ] Add `<IsAotCompatible>true</IsAotCompatible>` to project
- [ ] Add `<IsTrimmable>true</IsTrimmable>` to project
- [ ] Test with AOT publish to verify zero warnings
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

(Added after completion)
