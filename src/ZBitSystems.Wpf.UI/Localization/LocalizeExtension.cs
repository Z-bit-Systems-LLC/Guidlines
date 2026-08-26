using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace ZBitSystems.Wpf.UI.Localization;

/// <summary>
/// Markup extension for accessing localized resources in XAML.
/// Usage: {localization:Localize ResourceKey}
/// Requires LocalizationService.Provider to be set during application startup.
/// </summary>
[MarkupExtensionReturnType(typeof(string))]
public class LocalizeExtension : MarkupExtension
{
    /// <summary>
    /// Gets or sets the resource key.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Initializes a new instance of the LocalizeExtension.
    /// </summary>
    public LocalizeExtension()
    {
    }

    /// <summary>
    /// Initializes a new instance of the LocalizeExtension with a key.
    /// </summary>
    /// <param name="key">The resource key.</param>
    public LocalizeExtension(string key)
    {
        Key = key;
    }

    /// <summary>
    /// Provides the localized value or a binding for dynamic updates.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>The localized string value or binding.</returns>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (string.IsNullOrEmpty(Key))
            return "[MISSING_KEY]";

        // A Binding cannot be set on a plain CLR property such as Binding.StringFormat, so hand
        // back the current text instead of a binding that WPF would reject with a parse error.
        // Use LocalizeFormatExtension when formatted text has to follow culture changes.
        if (TargetRejectsBinding(serviceProvider))
            return GetStaticString();

        try
        {
            // Create a binding to the LocalizedStringBinding for dynamic updates
            var localizedBinding = new LocalizedStringBinding(Key);
            var binding = new Binding(nameof(LocalizedStringBinding.Value))
            {
                Source = localizedBinding,
                Mode = BindingMode.OneWay
            };

            return binding.ProvideValue(serviceProvider);
        }
        catch
        {
            // Fallback to static string if binding fails
            return GetStaticString();
        }
    }

    private static bool TargetRejectsBinding(IServiceProvider? serviceProvider) =>
        serviceProvider?.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget { TargetObject: BindingBase };

    private string GetStaticString()
    {
        try
        {
            return LocalizationService.Provider.GetString(Key);
        }
        catch
        {
            return $"[{Key}]";
        }
    }
}
