using System.Windows.Data;
using Moq;
using ZBitSystems.Wpf.UI.Localization;

namespace ZBitSystems.Wpf.UI.Tests.Localization;

[TestFixture]
public class LocalizeFormatExtensionTests
{
    [SetUp]
    public void SetUp()
    {
        var mockProvider = new Mock<ILocalizationProvider>();
        mockProvider.Setup(p => p.GetString("Test_Format")).Returns("Address {0} at {1}");
        mockProvider.Setup(p => p.GetString(It.Is<string>(key => key != "Test_Format")))
            .Returns((string key) => $"[{key}]");

        LocalizationService.Provider = mockProvider.Object;
    }

    [Test]
    public void Constructor_WithoutArguments_CreatesEmptyExtension()
    {
        // Act
        var extension = new LocalizeFormatExtension();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(extension.Key, Is.Empty);
            Assert.That(extension.Paths, Is.Empty);
        });
    }

    [Test]
    public void Constructor_WithKey_SetsKey()
    {
        // Act
        var extension = new LocalizeFormatExtension("Test_Format");

        // Assert
        Assert.That(extension.Key, Is.EqualTo("Test_Format"));
    }

    [Test]
    public void Constructor_WithKeyAndPaths_SetsBoth()
    {
        // Act
        var extension = new LocalizeFormatExtension("Test_Format", "First,Second");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(extension.Key, Is.EqualTo("Test_Format"));
            Assert.That(extension.Paths, Is.EqualTo("First,Second"));
        });
    }

    [Test]
    public void ProvideValue_WithEmptyKey_ReturnsMissingKeyPlaceholder()
    {
        // Arrange
        var extension = new LocalizeFormatExtension("");

        // Act
        var result = extension.ProvideValue(null!);

        // Assert
        Assert.That(result, Is.EqualTo("[MISSING_KEY]"));
    }

    [Test]
    public void ProvideValue_WithPaths_CreatesMultiBindingWithFormatFirst()
    {
        // Arrange
        var extension = new LocalizeFormatExtension("Test_Format", "First.Path,Second.Path");

        // Act
        var result = extension.ProvideValue(null!) as MultiBinding;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Converter, Is.InstanceOf<LocalizedFormatConverter>());
        Assert.That(result.Bindings, Has.Count.EqualTo(3));
        Assert.Multiple(() =>
        {
            // The localized format string is bound first so it follows culture changes
            Assert.That(((Binding)result.Bindings[0]).Source, Is.InstanceOf<LocalizedStringBinding>());
            Assert.That(((Binding)result.Bindings[1]).Path.Path, Is.EqualTo("First.Path"));
            Assert.That(((Binding)result.Bindings[2]).Path.Path, Is.EqualTo("Second.Path"));
        });
    }

    [Test]
    public void ProvideValue_WithoutPaths_CreatesMultiBindingWithFormatOnly()
    {
        // Arrange
        var extension = new LocalizeFormatExtension("Test_Format");

        // Act
        var result = extension.ProvideValue(null!) as MultiBinding;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Bindings, Has.Count.EqualTo(1));
    }

    [Test]
    public void ProvideValue_WithPaddedPaths_TrimsAndSkipsEmptyEntries()
    {
        // Arrange
        var extension = new LocalizeFormatExtension("Test_Format", " First.Path , , Second.Path ");

        // Act
        var result = extension.ProvideValue(null!) as MultiBinding;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Bindings, Has.Count.EqualTo(3));
        Assert.Multiple(() =>
        {
            Assert.That(((Binding)result.Bindings[1]).Path.Path, Is.EqualTo("First.Path"));
            Assert.That(((Binding)result.Bindings[2]).Path.Path, Is.EqualTo("Second.Path"));
        });
    }

    [Test]
    public void ProvideValue_BindsToCurrentLocalizedFormat()
    {
        // Arrange
        var extension = new LocalizeFormatExtension("Test_Format", "First.Path");

        // Act
        var result = (MultiBinding)extension.ProvideValue(null!);
        var formatSource = (LocalizedStringBinding)((Binding)result.Bindings[0]).Source!;

        // Assert
        Assert.That(formatSource.Value, Is.EqualTo("Address {0} at {1}"));
    }
}
