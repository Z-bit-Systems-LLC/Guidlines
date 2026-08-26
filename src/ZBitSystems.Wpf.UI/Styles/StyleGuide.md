# OSDP Bench UI Style Guide

## Overview
This style guide provides a comprehensive design system for OSDP Bench, ensuring visual consistency and maintainability across the application.

## Design System Structure

### 1. Design Tokens (`DesignTokens.xaml`)
Foundation values for spacing, typography, colors, and sizing.

#### Spacing System (8px grid)
```xml
{StaticResource Spacing.XSmall}    <!-- 4px -->
{StaticResource Spacing.Small}     <!-- 8px -->
{StaticResource Spacing.Medium}    <!-- 16px -->
{StaticResource Spacing.Large}     <!-- 24px -->
{StaticResource Spacing.XLarge}    <!-- 32px -->
{StaticResource Spacing.XXLarge}   <!-- 48px -->
```

#### Standard Margins & Padding
```xml
{StaticResource Margin.Card}          <!-- 10,5 -->
{StaticResource Margin.Control}       <!-- 0,0,16,0 -->
{StaticResource Margin.Button}        <!-- 8,4 -->
{StaticResource Margin.Section}       <!-- 0,0,0,16 -->
{StaticResource Margin.PageMessage}   <!-- 20,10,20,0 (centered page messages) -->
{StaticResource Margin.ContentCenter} <!-- 10,0,10,0 (horizontally centered content) -->
{StaticResource Padding.Card}         <!-- 16 -->
{StaticResource Padding.Control}      <!-- 8,4 -->
```

#### Typography Scale
```xml
{StaticResource FontSize.Caption}  <!-- 12px -->
{StaticResource FontSize.Body}     <!-- 14px -->
{StaticResource FontSize.BodyLarge} <!-- 16px -->
{StaticResource FontSize.Subtitle} <!-- 20px -->
{StaticResource FontSize.Title}    <!-- 24px -->
{StaticResource FontSize.Headline} <!-- 32px -->
{StaticResource FontSize.Display}  <!-- 36px -->
```

### 2. Component Styles (`ComponentStyles.xaml`)
Reusable styles for UI components.

#### Typography Usage
```xml
<!-- Page titles -->
<TextBlock Style="{StaticResource Page.Title}" Text="Page Name"/>

<!-- Section headers -->
<TextBlock Style="{StaticResource Text.Title}" Text="Section"/>

<!-- Body text -->
<TextBlock Style="{StaticResource Text.Body}" Text="Content"/>

<!-- Status messages -->
<TextBlock Style="{StaticResource Text.Error}" Text="Error message"/>
<TextBlock Style="{StaticResource Text.Warning}" Text="Warning"/>
<TextBlock Style="{StaticResource Text.Success}" Text="Success"/>

<!-- Page-level centered messages (e.g., disconnected/error states) -->
<TextBlock Style="{StaticResource Text.PageMessage}" Text="Device Not Connected"/>
<TextBlock Style="{StaticResource Text.PageMessage.Detail}" Text="Go to Connect page"/>

<!-- Statistics labels and values (e.g., dashboard counters) -->
<TextBlock Style="{StaticResource Text.Stat.Label}" Text="Commands Sent"/>
<TextBlock Style="{StaticResource Text.Stat.Value}" Text="1,234"/>
```

#### Form Controls
```xml
<!-- Labels -->
<Label Style="{StaticResource Label.Standard}" Content="Field Name"/>

<!-- Text boxes -->
<TextBox Style="{StaticResource TextBox.Standard}"/>

<!-- Read-only text boxes (includes gray background, copy button, disabled context menu) -->
<TextBox IsReadOnly="True" Style="{StaticResource TextBox.Standard}"/>

<!-- Combo boxes -->
<ComboBox Style="{StaticResource ComboBox.Standard}"/>

<!-- Number boxes -->
<ui:NumberBox Style="{StaticResource NumberBox.Standard}"/>
```

#### Validation
```xml
<!-- Validation error text (e.g., character count that turns red when invalid) -->
<TextBlock Style="{StaticResource Text.Validation.Error}" Text="16/32"/>

<!-- Recommended inline error border pattern using SemanticErrorBrush -->
<TextBox>
    <TextBox.Style>
        <Style TargetType="TextBox" BasedOn="{StaticResource TextBox.Standard}">
            <Style.Triggers>
                <DataTrigger Binding="{Binding IsInvalid}" Value="True">
                    <Setter Property="BorderBrush" Value="{DynamicResource SemanticErrorBrush}"/>
                    <Setter Property="BorderThickness" Value="2"/>
                </DataTrigger>
            </Style.Triggers>
        </Style>
    </TextBox.Style>
</TextBox>
```

**Validation Styles:**
- `Text.Validation.Error` - Caption-sized text in error color, suitable for field-level validation messages or character counts

#### Buttons
```xml
<!-- Primary actions -->
<ui:Button Style="{StaticResource Button.Primary}" Content="Save"/>

<!-- Secondary actions -->
<ui:Button Style="{StaticResource Button.Secondary}" Content="Cancel"/>

<!-- Transparent/subtle actions -->
<ui:Button Style="{StaticResource Button.Transparent}" Content="Browse"/>
```

#### Segmented Toggle Buttons
Use for switching between two or more mutually exclusive options (e.g., Temporary/Permanent, Discover/Manual). Uses `RadioButton` with a custom template for theme-aware styling.

```xml
<!-- Segmented toggle pair -->
<StackPanel Orientation="Horizontal">
    <RadioButton Content="Option A"
                 GroupName="MyGroup"
                 IsChecked="True"
                 Style="{StaticResource ToggleButton.Segmented}" />
    <RadioButton Content="Option B"
                 GroupName="MyGroup"
                 Style="{StaticResource ToggleButton.Segmented}"
                 Margin="4,0,0,0" />
</StackPanel>

<!-- With data binding -->
<StackPanel Orientation="Horizontal">
    <RadioButton Content="Discover"
                 IsChecked="{Binding IsDiscoverMode, Mode=TwoWay}"
                 GroupName="ConnectMode"
                 Style="{StaticResource ToggleButton.Segmented}" />
    <RadioButton Content="Manual"
                 IsChecked="{Binding IsDiscoverMode, Converter={StaticResource InverseBoolConverter}}"
                 GroupName="ConnectMode"
                 Style="{StaticResource ToggleButton.Segmented}"
                 Margin="4,0,0,0" />
</StackPanel>
```

**Visual States:**
- **Unselected**: Default control background with primary text
- **Hover**: Secondary fill background
- **Selected (checked)**: Light accent background (`AccentFillColorTertiaryBrush`)
- **Disabled**: 50% opacity

#### Badges
Status badges for displaying security states and status indicators.

```xml
<!-- Filled badges (white text on colored background) -->
<Border Style="{StaticResource Badge.Success}">
    <TextBlock Text="Encrypted" Style="{StaticResource Badge.Text}"/>
</Border>

<Border Style="{StaticResource Badge.Info}">
    <TextBlock Text="Monitor" Style="{StaticResource Badge.Text}"/>
</Border>

<Border Style="{StaticResource Badge.Warning}">
    <TextBlock Text="No Decryption" Style="{StaticResource Badge.Text}"/>
</Border>

<!-- Outlined badges (colored border/text on light background - better readability) -->
<Border Style="{StaticResource Badge.Error.Outlined}">
    <TextBlock Text="Clear Text" Style="{StaticResource Badge.Text.Error}"/>
</Border>

<!-- Small variants for data grids -->
<Border Style="{StaticResource Badge.Success.Small}">
    <TextBlock Text="Encrypted" Style="{StaticResource Badge.Text.Small}"/>
</Border>

<Border Style="{StaticResource Badge.Error.Outlined.Small}">
    <TextBlock Text="Clear Text" Style="{StaticResource Badge.Text.Error.Small}"/>
</Border>
```

**Badge Styles:**
- `Badge.Success` / `Badge.Success.Small` - Green filled (secure/positive states)
- `Badge.Info` / `Badge.Info.Small` - Blue filled (informational states)
- `Badge.Warning` / `Badge.Warning.Small` - Orange filled (warning states)
- `Badge.Error.Outlined` / `Badge.Error.Outlined.Small` - Red outlined (error/insecure states)

**Badge Text Styles:**
- `Badge.Text` / `Badge.Text.Small` - White text for filled badges
- `Badge.Text.Error` / `Badge.Text.Error.Small` - Red text for outlined error badges

### 3. Layout Templates (`LayoutTemplates.xaml`)
Templates for common layout patterns.

#### Page Structure
```xml
<!-- Page container -->
<StackPanel Style="{StaticResource Page.Container}">
    <!-- Page header with activity indicators -->
    <ContentControl Content="Page Title" 
                    Template="{StaticResource Template.PageHeader}"/>
    
    <!-- Content cards -->
    <ui:Card Style="{StaticResource Card.Standard}">
        <StackPanel Style="{StaticResource Card.Content}">
            <!-- Card content -->
        </StackPanel>
    </ui:Card>
</StackPanel>
```

#### Data Grids
```xml
<!-- Standard data grid -->
<DataGrid Style="{StaticResource DataGrid.Standard}">
    <!-- Columns -->
</DataGrid>

<!-- Monitor page data grid -->
<DataGrid Style="{StaticResource DataGrid.Monitor}">
    <!-- Columns -->
</DataGrid>
```

## Implementation Guidelines

### 1. Migration Strategy
1. **Immediate adoption**: Use new styles for all new components
2. **Gradual migration**: Update existing components when making changes
3. **Legacy support**: Existing `PageTitleStyle` remains functional

### 2. Best Practices

#### ✅ Do
- Use design tokens for spacing: `Margin="{StaticResource Margin.Card}"`
- Apply semantic color styles: `Style="{StaticResource Text.Error}"`
- Follow typography hierarchy: Display → Headline → Title → Subtitle → Body
- Use standard component styles for consistency
- Leverage layout templates for common patterns

#### ❌ Don't
- Use hardcoded margins/padding values
- Mix explicit FontSize with typography styles
- Use hardcoded colors (Red, Orange, etc.)
- Create one-off styles without considering reusability
- Override TextBox MinHeight in normal layouts (use design tokens instead)
- Use StaticResource syntax within Thickness strings (e.g., `"0,0,0,{StaticResource Spacing.Medium}"`)

#### Special Cases
```xml
<!-- Height-sensitive containers (DockPanel) - override TextBox MinHeight -->
<TextBox Style="{StaticResource TextBox.Standard}"
         MinHeight="0" Height="Auto" VerticalAlignment="Center"/>

<!-- DataGrid with invisible selection -->
<ui:DataGrid CanUserSortColumns="False">
    <ui:DataGrid.RowStyle>
        <Style TargetType="DataGridRow">
            <Style.Triggers>
                <Trigger Property="IsSelected" Value="True">
                    <Setter Property="Background" Value="Transparent"/>
                    <Setter Property="BorderBrush" Value="Transparent"/>
                </Trigger>
            </Style.Triggers>
        </Style>
    </ui:DataGrid.RowStyle>
    <ui:DataGrid.CellStyle>
        <Style TargetType="DataGridCell">
            <Style.Triggers>
                <Trigger Property="IsSelected" Value="True">
                    <Setter Property="Background" Value="Transparent"/>
                    <Setter Property="BorderBrush" Value="Transparent"/>
                    <Setter Property="Foreground" Value="{DynamicResource TextFillColorPrimaryBrush}"/>
                </Trigger>
            </Style.Triggers>
        </Style>
    </ui:DataGrid.CellStyle>
</ui:DataGrid>
```

### 3. Common Patterns

#### Form Layout
```xml
<StackPanel Style="{StaticResource Section.Container}">
    <ui:TextBlock Style="{StaticResource Section.Header}" 
                  Text="{markup:Localize Section_Title}"/>
    
    <!-- Form fields -->
    <ContentControl Tag="{markup:Localize Field_Label}"
                    Template="{StaticResource Template.FormField}">
        <TextBox Style="{StaticResource TextBox.Standard}"/>
    </ContentControl>
</StackPanel>
```

#### Status Messages
```xml
<TextBlock Text="{Binding StatusMessage}">
    <TextBlock.Style>
        <Style TargetType="TextBlock" BasedOn="{StaticResource Text.Body}">
            <Style.Triggers>
                <DataTrigger Binding="{Binding StatusLevel}" Value="Error">
                    <Setter Property="Foreground" Value="{StaticResource Brush.Error}"/>
                </DataTrigger>
                <!-- Additional triggers -->
            </Style.Triggers>
        </Style>
    </TextBlock.Style>
</TextBlock>
```

#### Tag Select (multi-select field)

For choosing several items from a list that is expected to grow. The field shows what has been
chosen as removable tags and keeps the full list behind a picker, so the section stays one row
tall however many options exist. Prefer it over a row of check boxes once the option list is long
enough to dominate the page, and over a plain list where what matters at a glance is what has been
chosen rather than what is on offer.

```xml
<Border Style="{StaticResource TagInput.Container}">
    <Grid>
        <!-- Column 0: placeholder shown when empty, plus a WrapPanel of Tag.Container chips -->
        <!-- Column 1: ToggleButton that opens a Popup holding the full ListBox.CheckList -->
    </Grid>
</Border>
```

Three WPF behaviours decide whether this works, and all three cost real debugging to find:

1. **The row owns the click, not the check box.** A themed `CheckBox` is only hit tested over the
   glyph and label it paints, so stretching one across a row does *not* widen the target. Bind the
   row's `IsSelected` to the item and make the check box a non-interactive indicator
   (`IsHitTestVisible="False"`, `Focusable="False"`) instead.
2. **Replace the row chrome, do not restyle it.** A themed list fills the whole row with the accent
   colour when an item is selected. On a list where most items are usually checked that reads as a
   solid block rather than a set of choices, so the item template is a plain `Border` with
   `Background="Transparent"` — transparent rather than unset, because an unpainted row is not hit
   tested at all.
3. **Take the opening control out of hit testing while the picker is open.** A `Popup` with
   `StaysOpen="False"` closes on any press outside it, and the toggle that opens it is outside it.
   Without this the same press that closes the picker also re-opens it, and the picker cannot be
   closed from the control that opened it. Bind `IsHitTestVisible` to the popup's inverted `IsOpen`
   rather than guarding on a timer: a timer window swallows a legitimate re-open.

Put bulk actions (select all, select none) inside the picker rather than on the page, where they
would compete with the page's primary button.

> **Where this lives today:** the styles (`TagInput.Container`, `Tag.Container`, `Tag.Text`,
> `Tag.Remove`, `TagInput.Placeholder`, `Popup.Surface`, `ListBox.CheckList` /
> `ListBoxItem.CheckList`) are in OSDP-Bench's `LayoutTemplates.xaml`, not in this library. See
> **Future Enhancements** for what promoting them would take.

### 4. Color System

#### Semantic Colors
- `{StaticResource Brush.Success}` - Green (#107C10)
- `{StaticResource Brush.Warning}` - Orange (#FF8C00)  
- `{StaticResource Brush.Error}` - Red (#D13438)
- `{StaticResource Brush.Info}` - Blue (#0078D4)

#### Activity Colors
- `{StaticResource Brush.Activity.Tx}` - Transmit activity
- `{StaticResource Brush.Activity.Rx}` - Receive activity
- `{StaticResource Brush.Activity.Inactive}` - Inactive state

#### Theme-Aware Colors (Recommended)
- `{DynamicResource TextFillColorPrimaryBrush}` - Primary text
- `{DynamicResource TextFillColorSecondaryBrush}` - Secondary text
- `{DynamicResource ControlFillColorDefaultBrush}` - Control backgrounds

## Controls

### LicenseExpander
A custom control for displaying license text in an expandable card with monospace formatting.

```xml
xmlns:controls="clr-namespace:ZBitSystems.Wpf.UI.Controls;assembly=ZBitSystems.Wpf.UI"

<controls:LicenseExpander Header="Apache License 2.0"
    ResourceUri="pack://application:,,,/Assets/Apache.txt" />
```

The control uses `ui:CardExpander` for the expandable container and renders license text in `Courier New` inside a `ScrollViewer`.

### InvertEffect
A pixel shader effect for inverting element colors, typically used to make logos visible in dark mode.

```xml
xmlns:effects="clr-namespace:ZBitSystems.Wpf.UI.Effects;assembly=ZBitSystems.Wpf.UI"

<Image Source="logo.png">
    <Image.Effect>
        <effects:InvertEffect />
    </Image.Effect>
</Image>
```

## Future Enhancements

### Planned Additions
1. **Animation system** - Consistent transitions and micro-interactions
2. **Responsive breakpoints** - Adaptive layouts for different window sizes
3. **Dark mode optimizations** - Enhanced dark theme color palette
4. **Accessibility styles** - High contrast and screen reader optimizations
5. **Tag select control** - promote the Tag Select pattern above into a lookless control here

### Promoting Tag Select

The pattern is documented under **Common Patterns** but is not a control in this library yet,
because it has only one consumer. Extract it on the second use, when the second caller is what
tells you which parts are the contract and which were incidental to the first. Moving it as it
stands would not be enough; three things have to be redesigned rather than copied:

- **It must not demand a shaped item type.** The styles currently bind `IsSelected`,
  `AccessibleName`, and `RemoveAccessibleName` on the data item, which forces every consumer's
  model into this library's shape. A control should take `ItemsSource` plus `SelectedItems` (or an
  `IsSelectedPath`) and `DisplayMemberPath`, and derive accessible names from the display text.
- **The behaviour must move with it.** Select-all, select-none, remove-one, and the is-empty state
  live on the consumer's view model today. They belong to the control, leaving the view model to
  own only the collection.
- **It needs a localization story.** This library ships no `.resx`; `ILocalizationProvider` leaves
  strings to the application. The picker's own chrome (select all, select none, the empty-state
  placeholder, the per-tag remove name) therefore needs either string dependency properties the
  application sets, or resources of its own here. Decide this deliberately - it is the first
  control in this library that has user-visible text of its own.

### Maintenance
- Review and update design tokens quarterly
- Add new component styles as needed
- Maintain backwards compatibility for existing styles
- Document any breaking changes in component updates

## Resources
- [WPF UI Documentation](https://wpfui.lepo.co/)
- [Microsoft Fluent Design](https://www.microsoft.com/design/fluent/)
- [Material Design System](https://material.io/design/introduction)