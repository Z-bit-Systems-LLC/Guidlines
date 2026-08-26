# ZBitSystems.Wpf.UI Usage Guide

This guide provides detailed instructions for using the shared WPF component library in your applications.

## Table of Contents
- [Getting Started](#getting-started)
- [Design System](#design-system)
- [Controls](#controls)
- [Converters](#converters)
- [Effects](#effects)
- [Helpers](#helpers)
- [Localization](#localization)
- [Settings Infrastructure](#settings-infrastructure)
- [Window State Management](#window-state-management)
- [Theme Management](#theme-management)
- [Components](#components)

## Getting Started

### Installation

Add as a git submodule:
```bash
git submodule add https://github.com/Z-bit-Systems-LLC/Guidelines.git lib/Guidelines
git submodule update --init --recursive
```

Add to your solution:
```bash
dotnet sln add lib/Guidelines/src/ZBitSystems.Wpf.UI/ZBitSystems.Wpf.UI.csproj
```

Add project reference to your WPF project's .csproj:
```xml
<ProjectReference Include="..\..\lib\Guidelines\src\ZBitSystems.Wpf.UI\ZBitSystems.Wpf.UI.csproj" />
```

## Design System

### Setting Up Styles

In your `App.xaml`, merge the design system resource dictionaries:

```xml
<Application x:Class="YourApp.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:ui="http://schemas.lepo.co/wpfui/2022/xaml">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <!-- WPF-UI Theme (required) -->
                <ui:ThemesDictionary Theme="Dark"/>
                <ui:ControlsDictionary/>

                <!-- Z-bit Systems Design System -->
                <ResourceDictionary Source="pack://application:,,,/ZBitSystems.Wpf.UI;component/Styles/DesignTokens.xaml"/>
                <ResourceDictionary Source="pack://application:,,,/ZBitSystems.Wpf.UI;component/Styles/ThemeSemanticColors.xaml"/>
                <ResourceDictionary Source="pack://application:,,,/ZBitSystems.Wpf.UI;component/Styles/ComponentStyles.xaml"/>
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

### Design Tokens

The design system provides centralized tokens for spacing, sizing, colors, and typography:

- **Spacing:** `{StaticResource Margin.Card}`, `{StaticResource Padding.Default}`, etc.
- **Sizes:** `{StaticResource Size.Icon.Default}`, `{StaticResource Size.Button.Height}`, etc.
- **Colors:** `{DynamicResource SemanticSuccessBrush}`, `{DynamicResource SemanticErrorBrush}`, etc.
- **Typography:** `{StaticResource Text.Title}`, `{StaticResource Text.Body}`, etc.

For detailed token reference, see `src/ZBitSystems.Wpf.UI/Styles/StyleGuide.md`.

## Controls

### LicenseExpander

A reusable control that displays license text inside an expandable card. License content can be set directly or loaded from a `pack://` resource URI.

#### Adding Namespace

```xml
xmlns:controls="clr-namespace:ZBitSystems.Wpf.UI.Controls;assembly=ZBitSystems.Wpf.UI"
```

#### Basic Usage

Load license text from an embedded resource:
```xml
<controls:LicenseExpander Header="Apache License 2.0"
    ResourceUri="pack://application:,,,/Assets/Apache.txt" />
```

Set license text directly:
```xml
<controls:LicenseExpander Header="MIT License"
    LicenseText="MIT License text here..." />
```

Constrain the content height with a scrollable area:
```xml
<controls:LicenseExpander Header="EPL License"
    ResourceUri="pack://application:,,,/Assets/EPL.txt"
    MaxContentHeight="400" />
```

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Header` | `string` | `""` | Header text displayed on the expander |
| `LicenseText` | `string` | `""` | License content to display |
| `ResourceUri` | `Uri?` | `null` | Pack URI to auto-load text from a resource |
| `MaxContentHeight` | `double` | `Infinity` | Maximum height of the scrollable content area |

## Effects

### InvertEffect

A pixel shader effect that inverts the colors of an element. Useful for making dark-on-light images visible in dark mode themes.

#### Adding Namespace

```xml
xmlns:effects="clr-namespace:ZBitSystems.Wpf.UI.Effects;assembly=ZBitSystems.Wpf.UI"
```

#### Basic Usage

Apply directly to an element:
```xml
<Image Source="logo.png">
    <Image.Effect>
        <effects:InvertEffect />
    </Image.Effect>
</Image>
```

Toggle based on dark mode (with ThemeManager):
```xml
<Image Source="logo.png">
    <Image.Style>
        <Style TargetType="Image">
            <Style.Triggers>
                <DataTrigger Binding="{Binding IsDarkMode}" Value="True">
                    <Setter Property="Effect">
                        <Setter.Value>
                            <effects:InvertEffect />
                        </Setter.Value>
                    </Setter>
                </DataTrigger>
            </Style.Triggers>
        </Style>
    </Image.Style>
</Image>
```

## Helpers

### ApplicationInfoHelper

Static helper for reading application metadata from assembly attributes.

```csharp
using ZBitSystems.Wpf.UI.Helpers;

// Get formatted version string (e.g., "Version 3.0.27")
string version = ApplicationInfoHelper.GetVersion();

// Get copyright notice from AssemblyCopyrightAttribute
string copyright = ApplicationInfoHelper.GetCopyright();

// Get product name from AssemblyProductAttribute
string product = ApplicationInfoHelper.GetProductName();
```

All methods default to `Assembly.GetEntryAssembly()` (the consuming application's assembly). Pass an explicit assembly to read from a different source:

```csharp
var assembly = typeof(MyClass).Assembly;
string version = ApplicationInfoHelper.GetVersion(assembly);
```

### CopyTextBoxHelper

See existing documentation for copy-to-clipboard attached property.

## Converters

The library provides generic value converters for common data binding scenarios.

### Adding Converter Namespace

Add this namespace to your XAML:
```xml
xmlns:converters="clr-namespace:ZBitSystems.Wpf.UI.Converters;assembly=ZBitSystems.Wpf.UI"
```

### Available Converters

#### BooleanToVisibilityConverter
Converts boolean values to Visibility. Supports inverting.

```xml
<Page.Resources>
    <converters:BooleanToVisibilityConverter x:Key="BoolToVis" />
</Page.Resources>

<!-- Show when true -->
<TextBlock Text="Visible when true"
           Visibility="{Binding IsEnabled, Converter={StaticResource BoolToVis}}" />

<!-- Show when false (inverted) -->
<TextBlock Text="Visible when false"
           Visibility="{Binding IsEnabled, Converter={StaticResource BoolToVis}, ConverterParameter=Invert}" />
```

#### InverseBoolConverter
Inverts boolean values.

```xml
<CheckBox IsChecked="{Binding IsDisabled, Converter={StaticResource InverseBool}}" />
```

#### NullToVisibilityConverter
Shows/hides elements based on null check.

```xml
<!-- Show when not null -->
<TextBlock Text="{Binding ErrorMessage}"
           Visibility="{Binding ErrorMessage, Converter={StaticResource NullToVis}}" />

<!-- Show when null (inverted) -->
<TextBlock Text="No errors"
           Visibility="{Binding ErrorMessage, Converter={StaticResource NullToVis}, ConverterParameter=Invert}" />
```

#### StringToVisibilityConverter
Shows element when string matches specific values (semicolon-separated).

```xml
<!-- Show when Status is "Active" or "Running" -->
<Border Visibility="{Binding Status, Converter={StaticResource StringToVis}, ConverterParameter='Active;Running'}">
    <TextBlock Text="Currently active" />
</Border>
```

#### IndexToVisibilityConverter
Shows element when index matches specific values (pipe-separated).

```xml
<!-- Show when SelectedIndex is 0 or 2 -->
<StackPanel Visibility="{Binding SelectedIndex, Converter={StaticResource IndexToVis}, ConverterParameter='0|2'}">
    <!-- Content -->
</StackPanel>
```

## Localization

The localization system provides an interface-based abstraction for multi-language support.

### Step 1: Implement ILocalizationProvider

Create a provider in your UI project that adapts your application's resource system to the `ILocalizationProvider` interface. The provider subscribes to culture-change notifications from your resources and re-raises them so the XAML bindings update automatically:

```csharp
using System.ComponentModel;
using System.Globalization;
using ZBitSystems.Wpf.UI.Localization;

public class ResourceLocalizationProvider : ILocalizationProvider
{
    public ResourceLocalizationProvider()
    {
        // Forward culture-change notifications from the static Resources class
        MyApp.Core.Resources.Resources.PropertyChanged += OnResourcesPropertyChanged;
    }

    public string GetString(string key)
    {
        return MyApp.Core.Resources.Resources.GetString(key);
    }

    public string CurrentCulture =>
        MyApp.Core.Resources.Resources.Culture?.Name ?? CultureInfo.CurrentUICulture.Name;

    private void OnResourcesPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
```

### Step 2: Register Provider at Startup

Set the provider after your host starts but before any UI is shown. If using the .NET Generic Host:

```csharp
using ZBitSystems.Wpf.UI.Localization;

private async void OnStartup(object sender, StartupEventArgs e)
{
    Host.Start();

    // Register the localization provider before any windows are created
    LocalizationService.Provider = new ResourceLocalizationProvider();

    // Apply saved culture preference
    var localizationService = Host.Services.GetService<ILocalizationService>();
    localizationService?.ChangeCulture(savedCulture);
}
```

### Step 3: Use in XAML

Add namespace:
```xml
xmlns:localization="clr-namespace:ZBitSystems.Wpf.UI.Localization;assembly=ZBitSystems.Wpf.UI"
```

Use the markup extension:
```xml
<TextBlock Text="{localization:Localize Welcome_Message}" />
<Button Content="{localization:Localize Button_Submit}" />
<TextBox Watermark="{localization:Localize Placeholder_EnterName}" />
```

### Formatted Strings

`StringFormat` is a plain CLR property, so it cannot receive the binding that `{localization:Localize}`
returns. Use `LocalizeFormat` instead - it keeps the format string bound, so the text still refreshes
when the culture changes and the placeholders can be reordered per language:

```xml
<!-- Resource: Status_ConnectionFormat = "Address {0} at {1}" -->
<TextBlock Text="{localization:LocalizeFormat Status_ConnectionFormat, Paths='ViewModel.ConnectedAddress,ViewModel.ConnectedBaudRate'}" />

<!-- Single argument -->
<TextBlock Text="{localization:LocalizeFormat Status_BaudRateFormat, Paths=ViewModel.BaudRate}" />
```

`Paths` is a comma separated list of binding paths, in the order the format string consumes them.

```xml
<!-- Not supported - WPF rejects a binding on StringFormat -->
<TextBlock Text="{Binding ViewModel.BaudRate, StringFormat={localization:Localize Status_BaudRateFormat}}" />
```

### Dynamic Language Switching

The bindings automatically update when the provider raises `PropertyChanged`:

```csharp
// Switch to Spanish
var spanishCulture = new CultureInfo("es");
localizationProvider.ChangeCulture(spanishCulture);
// All UI elements using {localization:Localize} will update automatically
```

## Settings Infrastructure

The library provides a shared settings infrastructure so all Z-bit apps use the same storage pattern.

### UserSettings Base Class

`UserSettings` implements `IWindowStateStorage` directly, eliminating the need for an adapter when using the built-in settings system:

```csharp
using ZBitSystems.Wpf.UI.Settings;

// Use directly for simple apps
services.AddSingleton<IUserSettingsService<UserSettings>>(
    new JsonUserSettingsService<UserSettings>("MyApp"));

// Or extend for app-specific settings
public class MyAppSettings : UserSettings
{
    public string PreferredCulture { get; set; } = "en-US";
    public bool AutoConnect { get; set; } = true;
}

services.AddSingleton<IUserSettingsService<MyAppSettings>>(
    new JsonUserSettingsService<MyAppSettings>("MyApp"));
```

### JsonUserSettingsService

Persists settings as JSON to `%LOCALAPPDATA%/{appName}/settings.json` (unpackaged) or the package-scoped local folder (MSIX). Settings are loaded synchronously on construction with automatic fallback to defaults.

### DI Registration

```csharp
// In App.xaml.cs ConfigureServices:
services.AddSingleton<IUserSettingsService<UserSettings>>(
    new JsonUserSettingsService<UserSettings>("CredBench"));
```

## Window State Management

The window state manager persists window position, size, and maximized state across application sessions.

### Recommended: Using UserSettings (No Adapter Needed)

Since `UserSettings` already implements `IWindowStateStorage`, you can pass it directly to `WindowStateManager`:

```csharp
using ZBitSystems.Wpf.UI.Services;
using ZBitSystems.Wpf.UI.Settings;

public partial class MainWindow : FluentWindow
{
    private readonly IUserSettingsService<UserSettings> _settingsService;
    private readonly WindowStateManager _windowStateManager;

    public MainWindow(MainViewModel viewModel, IUserSettingsService<UserSettings> settingsService)
    {
        InitializeComponent();
        DataContext = viewModel;

        _settingsService = settingsService;
        // UserSettings IS IWindowStateStorage — no adapter needed
        _windowStateManager = new WindowStateManager(this, settingsService.Settings);
        _windowStateManager.RestoreWindowState();
        Closing += OnClosing;
    }

    private async void OnClosing(object? sender, CancelEventArgs e)
    {
        _windowStateManager.SaveWindowState();
        await _settingsService.SaveAsync();
    }
}
```

### Alternative: Adapter Pattern

If your app stores settings in a Core project that cannot reference the Guidelines library, create an adapter in your UI project:

```csharp
using ZBitSystems.Wpf.UI.Services;

public class SettingsWindowStateAdapter(MySettings settings) : IWindowStateStorage
{
    public double WindowWidth { get => settings.WindowWidth; set => settings.WindowWidth = value; }
    public double WindowHeight { get => settings.WindowHeight; set => settings.WindowHeight = value; }
    public double? WindowLeft { get => settings.WindowLeft; set => settings.WindowLeft = value; }
    public double? WindowTop { get => settings.WindowTop; set => settings.WindowTop = value; }
    public bool IsMaximized { get => settings.IsMaximized; set => settings.IsMaximized = value; }
}
```

### Step 3: Handle DPI Changes

Override `OnDpiChanged` to fix rendering glitches when moving between monitors with different DPI settings:

```csharp
protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
{
    base.OnDpiChanged(oldDpi, newDpi);
    _windowStateManager.HandleDpiChanged();
}
```

This forces a visual refresh of the window and its content, preventing layout artifacts during DPI transitions.

### Features

The manager automatically handles:
- **Multi-monitor configurations** - Works correctly when displays are added/removed
- **DPI change handling** - Forces visual refresh during monitor DPI transitions
- **Position clamping** - Ensures windows remain accessible and visible
- **Smart centering** - Centers window when saved position is invalid
- **Normal bounds preservation** - Saves normal (non-maximized) bounds when window is maximized

## Theme Management

The theme manager automatically follows Windows OS theme settings (Light/Dark mode).

### Basic Usage

```csharp
using System.ComponentModel;
using ZBitSystems.Wpf.UI.Services;

public class MyPage : INavigableView, INotifyPropertyChanged, IDisposable
{
    private readonly ThemeManager _themeManager;

    public MyPage()
    {
        _themeManager = new ThemeManager();
        _themeManager.PropertyChanged += OnThemeChanged;

        InitializeComponent();
    }

    public bool IsDarkMode => _themeManager.IsDarkMode;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnThemeChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ThemeManager.IsDarkMode))
        {
            // Update UI when theme changes
            OnPropertyChanged(nameof(IsDarkMode));
        }
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void Dispose()
    {
        _themeManager.PropertyChanged -= OnThemeChanged;
        _themeManager.Dispose();
    }
}
```

### Using in XAML

React to theme changes with data binding:

```xml
<Page DataContext="{Binding RelativeSource={RelativeSource Self}}">
    <!-- Invert logo colors in dark mode -->
    <Image Source="logo.png">
        <Image.Style>
            <Style TargetType="Image">
                <Style.Triggers>
                    <DataTrigger Binding="{Binding IsDarkMode}" Value="True">
                        <Setter Property="Effect">
                            <Setter.Value>
                                <InvertEffect />
                            </Setter.Value>
                        </Setter>
                    </DataTrigger>
                </Style.Triggers>
            </Style>
        </Image.Style>
    </Image>

    <!-- Show different text based on theme -->
    <TextBlock>
        <TextBlock.Style>
            <Style TargetType="TextBlock">
                <Setter Property="Text" Value="Light Mode Active" />
                <Style.Triggers>
                    <DataTrigger Binding="{Binding IsDarkMode}" Value="True">
                        <Setter Property="Text" Value="Dark Mode Active" />
                    </DataTrigger>
                </Style.Triggers>
            </Style>
        </TextBlock.Style>
    </TextBlock>
</Page>
```

### Integration with MainWindow

For theme watching in your main window:

```csharp
public MainWindow()
{
    // Register for system theme changes
    // Note: This is already handled by WPF-UI's SystemThemeWatcher
    Wpf.Ui.Appearance.SystemThemeWatcher.Watch(this);

    InitializeComponent();
}
```

## Components

For detailed information about available component styles:

- **StyleGuide.md** - Complete style guide with examples
  - Available text styles (Title, Headline, Subtitle, Body, Caption)
  - Button styles (Primary, Secondary, Accent, Danger)
  - Card and container styles
  - Badge and status indicator styles
  - Form control styles
  - Best practices and usage guidelines

See: `src/ZBitSystems.Wpf.UI/Styles/StyleGuide.md`

## Architecture Best Practices

### Keep Core Projects UI-Independent

Core projects must never reference the Guidelines WPF library. All UI-specific types — including `UserSettings` and its extensions — belong in the UI project.

```
Core/           ← No WPF references. Models, services, ViewModels only.
UI/Windows/     ← References Core + Guidelines. Settings, adapters, views live here.
```

### Use Guidelines UserSettings in the UI Project

Use or extend `UserSettings` from the Guidelines library in your **UI project** (not Core). This gives you `IWindowStateStorage` for free without adapter boilerplate:

```csharp
// In UI/Windows project — NOT in Core
using ZBitSystems.Wpf.UI.Settings;

public class MyAppSettings : UserSettings
{
    public string PreferredCulture { get; set; } = "en-US";
}
```

### Adapter Pattern for Core-Owned Settings

If your Core project must own a platform-agnostic settings class, create an adapter in the UI project to bridge it with `IWindowStateStorage`:

```csharp
// Adapter in UI project — Core remains independent
public class SettingsWindowStateAdapter(CoreSettings settings) : IWindowStateStorage
{
    public double WindowWidth { get => settings.WindowWidth; set => settings.WindowWidth = value; }
    // ...
}
```
