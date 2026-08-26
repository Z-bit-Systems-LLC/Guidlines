# Z-bit Systems Guidelines

This repository contains shared company guidelines and reusable WPF components for Z-bit Systems projects.

## Contents

### Documentation (`docs/`)
- **azure-devops-ci-guide.md** - Guide for setting up Azure DevOps CI/CD pipelines
- **readme-template.md** - Template for creating project README files

### Shared WPF Library (`src/ZBitSystems.Wpf.UI/`)
A reusable WPF component library providing:
- **Design System** - Consistent styling and design tokens
- **Controls** - Reusable custom controls (LicenseExpander)
- **Converters** - Generic value converters for data binding
- **Effects** - Pixel shader effects (InvertEffect for dark mode)
- **Helpers** - Utility classes for common WPF scenarios
- **Localization** - Interface-based abstraction for multi-language support
- **Window Management** - State persistence with multi-monitor support
- **Theme Management** - Automatic Windows system theme following
- Built on WPF-UI 4.2.0
- Targets .NET 10.0-windows
- Comprehensive unit test coverage (98 tests)

#### Features

**Controls:**
- **LicenseExpander** - Expandable card for displaying license text, with automatic resource loading from pack:// URIs

**Effects:**
- **InvertEffect** - Pixel shader that inverts element colors, useful for making dark-on-light images visible in dark themes

**Design System:**
- **Design Tokens** - Centralized spacing, sizing, colors, and typography
- **Semantic Colors** - Theme-aware color system with automatic light/dark mode support
- **Component Styles** - Pre-styled controls (buttons, textboxes, cards, badges, etc.)

**Converters:**
- **BooleanToVisibilityConverter** - Convert bool to Visibility with optional invert parameter
- **InverseBoolConverter** - Invert boolean values
- **NullToVisibilityConverter** - Show/hide based on null check
- **StringToVisibilityConverter** - Show/hide based on string match (supports semicolon-separated values)
- **IndexToVisibilityConverter** - Show/hide based on index match (supports pipe-separated indices)

**Helpers:**
- **ApplicationInfoHelper** - Static methods for reading version, copyright, and product name from assembly attributes
- **CopyTextBoxHelper** - Attached property for copy-to-clipboard functionality on read-only textboxes
- **RelayCommand** - Simple ICommand implementation for MVVM patterns

**Localization:**
- **ILocalizationProvider** - Interface for implementing custom resource providers
- **LocalizationService** - Static service for managing the localization provider
- **LocalizeExtension** - XAML markup extension for accessing localized resources
- **LocalizeFormatExtension** - XAML markup extension for formatting bound values with a localized format string
- **LocalizedFormatConverter** - Multi-value converter that applies a bound composite format string
- **LocalizedStringBinding** - Dynamic binding that updates when culture changes

**Window Management:**
- **IWindowStateStorage** - Interface for implementing window state persistence
- **WindowStateManager** - Manages window position, size, and maximized state
  - Multi-monitor support with smart positioning
  - Ensures windows remain accessible across monitor configuration changes
  - DPI change handling for smooth multi-monitor transitions

**Theme Management:**
- **ThemeManager** - Automatically follows Windows OS theme settings
  - Provides `IsDarkMode` property for theme-aware UI
  - Raises `PropertyChanged` events when system theme changes
  - Built on WPF-UI's ApplicationThemeManager

## Quick Start

### Installation

Add as a git submodule:
```bash
git submodule add https://github.com/Z-bit-Systems-LLC/Guidelines.git lib/Guidelines
git submodule update --init --recursive
```

Add to your solution and project:
```bash
dotnet sln add lib/Guidelines/src/ZBitSystems.Wpf.UI/ZBitSystems.Wpf.UI.csproj
```

```xml
<!-- Add to your WPF project's .csproj -->
<ProjectReference Include="..\..\lib\Guidelines\src\ZBitSystems.Wpf.UI\ZBitSystems.Wpf.UI.csproj" />
```

### Basic Setup

Merge design system resources in your `App.xaml`:
```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ui:ThemesDictionary Theme="Dark"/>
            <ui:ControlsDictionary/>
            <ResourceDictionary Source="pack://application:,,,/ZBitSystems.Wpf.UI;component/Styles/DesignTokens.xaml"/>
            <ResourceDictionary Source="pack://application:,,,/ZBitSystems.Wpf.UI;component/Styles/ThemeSemanticColors.xaml"/>
            <ResourceDictionary Source="pack://application:,,,/ZBitSystems.Wpf.UI;component/Styles/ComponentStyles.xaml"/>
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

## Documentation

📖 **[Complete Usage Guide](docs/WPF-UI-Usage-Guide.md)** - Comprehensive documentation covering:
- Design System integration
- Converters usage
- Localization setup
- Window State Management
- Theme Management
- Component styles and best practices

## Development

### Building
```bash
dotnet build Guidelines.slnx
```

### Testing
Run unit tests:
```bash
dotnet test
```

Test project: `test/ZBitSystems.Wpf.UI.Tests/`
- Framework: NUnit 4.3.2
- Mocking: Moq 4.20.72
- Coverage: 98 tests covering controls, converters, effects, helpers, localization, window management, and theme management

### Versioning
Version is managed in `Directory.Build.props`. Update `VersionPrefix` for new releases.

## License
Apache License 2.0

## Contributing
This is an internal Z-bit Systems repository. For questions or contributions, contact the development team.