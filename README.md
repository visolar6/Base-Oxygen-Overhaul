# Base Oxygen Overhaul

A Subnautica mod that overhauls the base oxygen system by introducing oxygen generators, consumption mechanics, and production management.

## Overview

This mod transforms Subnautica's base oxygen mechanics by making oxygen a managed resource rather than an infinite supply. Players must install oxygen generators in their bases and ensure production exceeds consumption based on base size and complexity.

### Key Features

- **Oxygen Generators**: Two new buildable oxygen generators
  - **Air Synthesizer**: Compact, low-capacity generator for small bases
  - **Atmospheric Processing Unit**: High-capacity generator for large bases
- **Dynamic Oxygen Consumption**: Each base compartment consumes oxygen based on size
- **Production/Consumption Balance**: Players must maintain positive net oxygen production
- **Partial Oxygen Loss**: Optional proportional oxygen depletion based on production deficit
- **Base Snorkel System**: Optional infinite oxygen for bases with above-water habitable sections
- **Multi-language Support**: 12 languages including English, Spanish, French, German, Russian, Japanese, Korean, Chinese, Portuguese, Polish, Italian, and Turkish
- **Audio/Visual Feedback**: Custom sounds and status displays for generators

## Requirements

### Runtime Dependencies
- **Subnautica**: Compatible with the latest version
- **BepInEx**: Mod loader framework
- **Nautilus**: Subnautica modding framework

### Development Dependencies
- **.NET SDK 8.0.417+**: For building the project
- **C# 7.3**: Language version used
- **.NET Framework 4.7.2**: Target framework (matches Subnautica)
- **Unity 2019**: Asset bundling

## Building

### Quick Build

```bash
./build.sh
```

The build script will:
1. Compile the project in Release mode
2. Copy DLL and assets to `bin/Release/net472/`
3. Create a distribution package in `bin/built/`
4. Optionally deploy to Subnautica's BepInEx plugins folder

### Configuration

Set the `SUBNAUTICA_PATH` environment variable to enable automatic deployment:

```bash
set SUBNAUTICA_PATH=C:\Program Files\Steam\steamapps\common\Subnautica
```

### Build Targets

```bash
# Debug build (includes debug symbols)
dotnet build -c Debug

# Release build (optimized)
dotnet build -c Release

# Clean build artifacts
dotnet clean
```

## Configuration Options

Configurable through in-game mod options menu:

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `ProductionRateSmallOxygenGenerator` | Slider | 0.5 | Air synthesizer oxygen/second |
| `ProductionRateLargeOxygenGenerator` | Slider | 4.0 | Atmospheric processing unit oxygen/second |
| `ShowNoOxygenGeneratorWarnings` | Toggle | true | Display warnings when base has no generators |
| `PartialOxygenLoss` | Toggle | true | Proportional oxygen loss vs. maximum loss |
| `AllowBaseSnorkel` | Toggle | false | Infinite oxygen for bases with above-water sections |

## Development Workflow

### Adding New Features

1. **Create feature branch**
   ```bash
   git checkout -b feature/my-feature
   ```

2. **Implement changes**
   - Add new classes in appropriate directories
   - Update `Plugin.cs` if registration is needed
   - Add translations to `Localizations.xml`

3. **Build and test**
   ```bash
   ./build.sh
   ```

4. **Test in-game**
   - Launch Subnautica
   - Check BepInEx console for errors
   - Verify functionality

### Adding Translations

Edit `Localizations.xml` and add entries for all 12 supported languages:
```xml
<LocalizationPackage Lang="English">
    <Text key="YourKey">Your English Text</Text>
</LocalizationPackage>
<LocalizationPackage Lang="Spanish">
    <Text key="YourKey">Tu Texto en Español</Text>
</LocalizationPackage>
<!-- ... other languages ... -->
```

### Debugging

1. **Enable debug build**
   ```bash
   dotnet build -c Debug
   ```

2. **View logs**
   - In-game: Press F8 to open BepInEx console
   - File: `Subnautica/BepInEx/LogOutput.log`

3. **Use logging**
   ```csharp
   Plugin.Log?.LogInfo("Debug message");
   Plugin.Log?.LogWarning("Warning message");
   Plugin.Log?.LogError("Error message");
   ```

## Performance Considerations

The mod is optimized to run efficiently:

- **Frame-by-frame evaluation**: Oxygen gain/loss checks run every frame but use cached values
- **Interval-based updates**: Expensive calculations (net rate, habitable cells) update every 2-3 seconds
- **Component caching**: Manager references and localized strings are cached in MonoBehaviours
- **Early returns**: Quick checks before expensive operations

### Key Timers
- `BaseOxygenDepletionInterval`: 3s - How often oxygen is depleted
- `BaseNetRateCheckInterval`: 2s - How often production/consumption is recalculated
- `HabitableCellCheckInterval`: 3s - How often above-water checks happen
- `NoOxygenGeneratorWarningInterval`: 10s - How often warnings are shown

## Testing

### Manual Testing Checklist

- [ ] Build small base, verify warning appears without generators
- [ ] Install small oxygen generator, verify oxygen production
- [ ] Hand hover over generator, verify hand text and subtext displays
- [ ] Expand base beyond generator capacity, verify oxygen depletion
<!-- - [ ] Install large generator, verify increased production -->
- [ ] Build base section above water, verify snorkel behavior (with option enabled)
- [ ] Remove base power, verify max oxygen depletion and generators display "OFFLINE"
- [ ] Flood base section, verify generators display "FLOODED"
- [ ] Check all languages load correctly
- [ ] Verify mod options save/load properly

### Unit Tests

```bash
./test.sh
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

### Code Style
- Use C# naming conventions (PascalCase for public members)
- Add XML documentation comments for public APIs
- Keep methods focused and under 50 lines when possible
- Use meaningful variable names

### Commit Messages
```
feat: Add new oxygen warning system
fix: Correct timer reset on base entry
docs: Update README with configuration details
refactor: Simplify habitable cell checking
```

## License

See `LICENSE` file for details.

## Acknowledgments

- **Nautilus Team**: For the excellent modding framework
- **BepInEx Team**: For the plugin loader
- **Subnautica Modding Community**: For tools, support, and discussions
- **AndreaDev3d**: For the Air Synthesizer model and textures