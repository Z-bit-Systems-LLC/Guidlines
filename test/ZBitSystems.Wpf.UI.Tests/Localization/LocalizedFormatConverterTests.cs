using System.Globalization;
using System.Windows;
using ZBitSystems.Wpf.UI.Localization;

namespace ZBitSystems.Wpf.UI.Tests.Localization;

[TestFixture]
public class LocalizedFormatConverterTests
{
    private LocalizedFormatConverter _converter = null!;

    [SetUp]
    public void SetUp()
    {
        _converter = new LocalizedFormatConverter();
    }

    [Test]
    public void Convert_WithFormatAndArguments_ReturnsFormattedString()
    {
        // Act
        var result = _converter.Convert(["Address {0} at {1}", 3, 9600], typeof(string), null,
            CultureInfo.InvariantCulture);

        // Assert
        Assert.That(result, Is.EqualTo("Address 3 at 9600"));
    }

    [Test]
    public void Convert_WithReorderedPlaceholders_ReturnsFormattedString()
    {
        // Act - translations are free to reorder the arguments
        var result = _converter.Convert(["{1} baud, address {0}", 3, 9600], typeof(string), null,
            CultureInfo.InvariantCulture);

        // Assert
        Assert.That(result, Is.EqualTo("9600 baud, address 3"));
    }

    [Test]
    public void Convert_WithNoArguments_ReturnsFormatUnchanged()
    {
        // Act
        var result = _converter.Convert(["No placeholders here"], typeof(string), null,
            CultureInfo.InvariantCulture);

        // Assert
        Assert.That(result, Is.EqualTo("No placeholders here"));
    }

    [Test]
    public void Convert_WithNoValues_ReturnsEmptyString()
    {
        // Act
        var result = _converter.Convert([], typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.That(result, Is.EqualTo(string.Empty));
    }

    [Test]
    public void Convert_WithUnresolvedFormat_ReturnsEmptyString()
    {
        // Act - the format binding has not produced a value yet
        var result = _converter.Convert([DependencyProperty.UnsetValue, 9600], typeof(string), null,
            CultureInfo.InvariantCulture);

        // Assert
        Assert.That(result, Is.EqualTo(string.Empty));
    }

    [Test]
    public void Convert_WithUnresolvedArgument_TreatsArgumentAsNull()
    {
        // Act
        var result = _converter.Convert(["Address {0} at {1}", 3, DependencyProperty.UnsetValue], typeof(string), null,
            CultureInfo.InvariantCulture);

        // Assert
        Assert.That(result, Is.EqualTo("Address 3 at "));
    }

    [Test]
    public void Convert_WithMalformedFormat_ReturnsFormatUnchanged()
    {
        // Act - a bad resource string should not take down the visual tree
        var result = _converter.Convert(["Address {0 at {1}", 3, 9600], typeof(string), null,
            CultureInfo.InvariantCulture);

        // Assert
        Assert.That(result, Is.EqualTo("Address {0 at {1}"));
    }

    [Test]
    public void Convert_UsesSuppliedCulture()
    {
        // Act
        var result = _converter.Convert(["{0}", 1234.5], typeof(string), null, new CultureInfo("de-DE"));

        // Assert
        Assert.That(result, Is.EqualTo("1234,5"));
    }

    [Test]
    public void ConvertBack_Throws()
    {
        // Act & Assert
        Assert.That(() => _converter.ConvertBack("text", [typeof(string)], null, CultureInfo.InvariantCulture),
            Throws.InstanceOf<NotSupportedException>());
    }
}
