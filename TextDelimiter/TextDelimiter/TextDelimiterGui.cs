using System.ComponentModel.Composition;
using DevToys.Api;
using TextDelimiter.Transformations;
using static DevToys.Api.GUI;

namespace TextDelimiter;

[Export(typeof(IGuiTool))]
[Name("TextDelimiter")] // A unique, internal name of the tool.
[ToolDisplayInformation(
    IconFontName = "FluentSystemIcons", // This font is available by default in DevToys
    IconGlyph = '\uE075', // An icon that represents a pizza
    GroupName = PredefinedCommonToolGroupNames.Text, // The group in which the tool will appear in the side bar.
    ResourceManagerAssemblyIdentifier = nameof(ResourceAssemblyIdentifier), // The Resource Assembly Identifier to use
    ResourceManagerBaseName = "TextDelimiter.TextDelimiter", // The full name (including namespace) of the resource file containing our localized texts
    ShortDisplayTitleResourceName = nameof(TextDelimiter.ShortDisplayTitle), // The name of the resource to use for the short display title
    LongDisplayTitleResourceName = nameof(TextDelimiter.LongDisplayTitle),
    DescriptionResourceName = nameof(TextDelimiter.Description),
    AccessibleNameResourceName = nameof(TextDelimiter.AccessibleName)
)]
internal sealed class TextDelimiterGui : IGuiTool
{
    private UIToolView? _view;

    [Import]
    private ISettingsProvider _settingsProvider = null!;

    private ExplodeMode _explodeMode = ExplodeMode.NewLines;

    #region settingDefinitions
    private readonly SettingDefinition<bool> _tidyUp = new(name: "tidy-up", defaultValue: false);
    private readonly SettingDefinition<bool> _attackTheClones =
        new(name: "attack-the-clones", defaultValue: false);

    private readonly SettingDefinition<ExplodeMode> _explodeModeSetting =
        new(name: "explode-mode", defaultValue: ExplodeMode.NewLines);

    #endregion


    #region UIElements

    private readonly IUIMultiLineTextInput _input = MultiLineTextInput().Title(TextDelimiter.Input);

    private readonly IUIMultiLineTextInput _output = MultiLineTextInput()
        .Title(TextDelimiter.Output);
    private readonly IUISelectDropDownList _delimiter = SelectDropDownList("delimiter")
        .Title(TextDelimiter.NewDelimiter)
        .AlignHorizontally(UIHorizontalAlignment.Left)
        .WithItems(
            Item(text: TextDelimiter.Comma, value: Delimiter.Comma),
            Item(text: TextDelimiter.Spaces, value: Delimiter.Spaces),
            Item(text: TextDelimiter.NewLines, value: Delimiter.NewLines),
            Item(text: TextDelimiter.Semicolon, value: Delimiter.Semicolon),
            Item(text: TextDelimiter.Pipe, value: Delimiter.Pipe),
            Item(text: TextDelimiter.Custom, value: Delimiter.Custom)
        )
        .Select(0);

    private readonly IUISingleLineTextInput _customDelimiter;

    private readonly IUISingleLineTextInput _openTag;
    private readonly IUISingleLineTextInput _closeTag;

    public TextDelimiterGui()
    {
        _customDelimiter = SingleLineTextInput("custom-delimiter")
            .Title(TextDelimiter.CustomDelimiter);
        _customDelimiter.Hide();
        _openTag = SingleLineTextInput("open-tag").Title(TextDelimiter.OpenTag).HideCommandBar();
        _closeTag = SingleLineTextInput("close-tag").Title(TextDelimiter.CloseTag).HideCommandBar();
    }

    #endregion


    #region enums
    enum GridRows
    {
        ConfigRow,
        AppRow,
        TransformRow,
        CreditRow
    }

    private enum GridColumns
    {
        Stretch
    }

    #endregion

    #region events


    private void OnDelimiterChanged(IUIDropDownListItem? selectedItem)
    {
        if (selectedItem == null || selectedItem.Value == null)
        {
            return;
        }
        Delimiter delimiter = (Delimiter)selectedItem.Value;
        if (delimiter == Delimiter.Custom)
        {
            _customDelimiter.Show();
        }
        else
        {
            _customDelimiter.Hide();
        }
    }

    public void OnDataReceived(string dataTypeName, object? parsedData)
    {
        throw new NotImplementedException();
    }

    private void onExplodeModeChanged(ExplodeMode mode)
    {
        _explodeMode = mode;
    }

    private ValueTask onTransformButtonClicked()
    {
        string input = _input.Text;
        if (_delimiter.SelectedItem == null || _delimiter.SelectedItem.Value == null)
        {
            return ValueTask.FromException(new InvalidOperationException("No delimiter selected"));
        }

        Delimiter delimiter = (Delimiter)_delimiter.SelectedItem.Value;
        string delimiterString = _customDelimiter.Text;

        if (delimiter != Delimiter.Custom)
        {
            delimiterString = ((char)delimiter).ToString();
        }

        List<ITextTransformer> transformations = getTransformations();

        TextDelimiterTransformer transformer =
            new(
                _explodeMode,
                delimiterString,
                transformations.OrderBy(transformation => transformation.Order).ToList()
            );

        ReadOnlyMemory<char> inputMemory = input.AsMemory();
        string output = transformer.DelimitText(inputMemory);

        _output.Text(output);

        return ValueTask.CompletedTask;
    }

    private List<ITextTransformer> getTransformations()
    {
        List<ITextTransformer> transformations = [];

        if (_settingsProvider.GetSetting(_attackTheClones))
        {
            transformations.Add(new AttackTheClonesTransformer());
        }

        if (_settingsProvider.GetSetting(_tidyUp))
        {
            transformations.Add(new TidyUpTransformer());
        }

        if (!string.IsNullOrEmpty(_openTag.Text) || !string.IsNullOrEmpty(_closeTag.Text))
        {
            transformations.Add(new WrapperTransformer(_openTag.Text, _closeTag.Text));
        }

        return transformations;
    }
    #endregion

    public UIToolView View
    {
        get
        {
            _view ??= new UIToolView(
                Grid()
                    .Rows(
                        (GridRows.ConfigRow, Auto),
                        (GridRows.AppRow, new UIGridLength(1, UIGridUnitType.Fraction)),
                        (GridRows.TransformRow, Auto),
                        (GridRows.CreditRow, Auto)
                    )
                    .Columns((GridColumns.Stretch, new UIGridLength(1, UIGridUnitType.Fraction)))
                    .Cells(
                        Cell(
                            GridRows.ConfigRow,
                            GridColumns.Stretch,
                            Stack()
                                .Vertical()
                                .WithChildren(
                                    Stack()
                                        .Vertical()
                                        .WithChildren(
                                            _delimiter.OnItemSelected(OnDelimiterChanged),
                                            _customDelimiter
                                        ),
                                    SettingGroup("")
                                        .Icon("FluentSystemIcons", '\uF6A9')
                                        .Title("Settings")
                                        .WithChildren(
                                            Setting()
                                                .Title(TextDelimiter.ExplodeMode)
                                                .Description(TextDelimiter.ExplodeModeDescription)
                                                .Handle(
                                                    _settingsProvider,
                                                    _explodeModeSetting,
                                                    onExplodeModeChanged,
                                                    Item(
                                                        text: TextDelimiter.NewLines,
                                                        value: ExplodeMode.NewLines
                                                    ),
                                                    Item(
                                                        text: TextDelimiter.Spaces,
                                                        value: ExplodeMode.Spaces
                                                    ),
                                                    Item(text: ",", value: ExplodeMode.Commas),
                                                    Item(text: ";", value: ExplodeMode.Semicolons)
                                                ),
                                            Setting()
                                                .Title(TextDelimiter.AttackTheClones)
                                                .Description(TextDelimiter.AttackTheClonesOn)
                                                .Handle(_settingsProvider, _attackTheClones),
                                            Setting()
                                                .Title(TextDelimiter.TidyUp)
                                                .Description(TextDelimiter.TidyUpOn)
                                                .Handle(_settingsProvider, _tidyUp),
                                            _openTag,
                                            _closeTag
                                        )
                                )
                        ),
                        Cell(
                            GridRows.AppRow,
                            GridColumns.Stretch,
                            SplitGrid()
                                .Vertical()
                                .WithLeftPaneChild(_input)
                                .WithRightPaneChild(_output.ReadOnly())
                        ),
                        Cell(
                            GridRows.TransformRow,
                            GridColumns.Stretch,
                            Button()
                                .AccentAppearance()
                                .Icon("FluentSystemIcons", '\uEE37')
                                .Text(TextDelimiter.Transform)
                                .OnClick(onTransformButtonClicked)
                        ),
                        Cell(
                            GridRows.CreditRow,
                            GridColumns.Stretch,
                            Label().Text("Made by benyaa | Based on delim.co")
                        )
                    )
            );
            return _view;
        }
    }
}
