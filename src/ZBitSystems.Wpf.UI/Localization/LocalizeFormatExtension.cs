using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace ZBitSystems.Wpf.UI.Localization;

/// <summary>
/// Markup extension that formats bound values with a localized composite format string.
/// Usage: {localization:LocalizeFormat ResourceKey, Paths='First.Path,Second.Path'}
/// Use this instead of StringFormat={localization:Localize Key}, which WPF rejects because
/// StringFormat is a plain CLR property and cannot receive a binding.
/// Requires LocalizationService.Provider to be set during application startup.
/// </summary>
[MarkupExtensionReturnType(typeof(string))]
public class LocalizeFormatExtension : MarkupExtension
{
    /// <summary>
    /// Gets or sets the resource key of the composite format string.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the comma separated binding paths supplying the format arguments,
    /// in the order they appear in the format string.
    /// </summary>
    public string Paths { get; set; } = string.Empty;

    /// <summary>
    /// Initializes a new instance of the LocalizeFormatExtension.
    /// </summary>
    public LocalizeFormatExtension()
    {
    }

    /// <summary>
    /// Initializes a new instance of the LocalizeFormatExtension with a key.
    /// </summary>
    /// <param name="key">The resource key of the composite format string.</param>
    public LocalizeFormatExtension(string key)
    {
        Key = key;
    }

    /// <summary>
    /// Initializes a new instance of the LocalizeFormatExtension with a key and argument paths.
    /// </summary>
    /// <param name="key">The resource key of the composite format string.</param>
    /// <param name="paths">The comma separated binding paths supplying the format arguments.</param>
    public LocalizeFormatExtension(string key, string paths)
    {
        Key = key;
        Paths = paths;
    }

    /// <summary>
    /// Provides a multi binding that formats the bound values with the localized format string.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>The formatted string binding.</returns>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (string.IsNullOrEmpty(Key))
            return "[MISSING_KEY]";

        var multiBinding = new MultiBinding
        {
            Mode = BindingMode.OneWay,
            Converter = new LocalizedFormatConverter()
        };

        // The format string is the first bound value so the text refreshes on a culture change
        multiBinding.Bindings.Add(CreateFormatBinding());

        foreach (var path in Paths.Split(','))
        {
            var trimmedPath = path.Trim();
            if (trimmedPath.Length == 0) continue;

            multiBinding.Bindings.Add(new Binding(trimmedPath) { Mode = BindingMode.OneWay });
        }

        return multiBinding.ProvideValue(serviceProvider);
    }

    private Binding CreateFormatBinding()
    {
        try
        {
            return new Binding(nameof(LocalizedStringBinding.Value))
            {
                Source = new LocalizedStringBinding(Key),
                Mode = BindingMode.OneWay
            };
        }
        catch
        {
            // Fallback to a static format string if the provider is unavailable
            return new Binding { Source = GetStaticFormat(), Mode = BindingMode.OneWay };
        }
    }

    private string GetStaticFormat()
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
