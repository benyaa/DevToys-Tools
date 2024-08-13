namespace JSONSchema;

using System.ComponentModel.Composition;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Serialization;
using CommunityToolkit.Diagnostics;
using DevToys.Api;
using NJsonSchema;
using NJsonSchema.CodeGeneration;
using static DevToys.Api.GUI;

[Export(typeof(IGuiTool))]
[Name("JSONSchemaClassGenerator")] // A unique, internal name of the tool.
[ToolDisplayInformation(
    IconFontName = "DevToys-Tools-Icons", // This font is available by default in DevToys
    IconGlyph = '\u0108', // An icon that represents a pizza
    GroupName = PredefinedCommonToolGroupNames.Generators, // The group in which the tool will appear in the side bar.
    ResourceManagerAssemblyIdentifier = nameof(ResourceAssemblyIdentifier), // The Resource Assembly Identifier to use
    ResourceManagerBaseName = "JSONSchema.ClassGenerator.JSONSchemaClassGenerator", // The full name (including namespace) of the resource file containing our localized texts
    ShortDisplayTitleResourceName = nameof(JSONSchemaClassGenerator.ShortDisplayTitle), // The name of the resource to use for the short display title
    LongDisplayTitleResourceName = nameof(JSONSchemaClassGenerator.LongDisplayTitle),
    DescriptionResourceName = nameof(JSONSchemaClassGenerator.Description),
    AccessibleNameResourceName = nameof(JSONSchemaClassGenerator.AccessibleName)
)]
[AcceptedDataTypeName(PredefinedCommonDataTypeNames.Json)]
internal sealed class JSONSchemaClassGeneratorGui : IGuiTool
{
    private UIToolView? _view;

    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    [Import]
    private ISettingsProvider _settingsProvider = null!;

    private readonly SettingDefinition<ClassGenerationType> _classGenerationType =
        new(name: "class-generation-type", defaultValue: ClassGenerationType.TypeScript);

    public JSONSchemaClassGeneratorGui()
    {
        _input = MultiLineTextInput()
            .Title(JSONSchemaClassGenerator.Input)
            .Language("json")
            .OnTextChanged(triggerValidation);

        _output = MultiLineTextInput()
            .Title(JSONSchemaGenerator.Schema)
            .Language("typescript")
            .ReadOnly();

        _inputFile = FileSelector("input-file")
            .CanSelectOneFile()
            .OnFilesSelected(onInputFileSelected)
            .LimitFileTypesTo(".json");

        _defaultError = getGeneralErrorInfoBar(JSONSchemaGenerator.JsonRequiredError);
    }

    #region enums

    enum ClassGenerationType
    {
        CSharp,
        TypeScript
    }

    enum GridRows
    {
        ConfigRow,
        UploadRow,
        InputRow,
        ErrorsRow
    }

    private enum GridColumns
    {
        Stretch
    }

    #endregion

    #region events

    private void onOutputTypeChanged(ClassGenerationType type)
    {
        generateClass();
    }

    private async ValueTask onFileSelected(IUIMultiLineTextInput input, SandboxedFileReader[] files)
    {
        Guard.HasSizeEqualTo(files, 1);
        using var memStream = new MemoryStream();
        using var file = files[0];

        await file.CopyFileContentToAsync(memStream, CancellationToken.None);
        var str = Encoding.UTF8.GetString(memStream.ToArray());
        input.Text(prettifyAsJsonOrDoNothing(str));
    }

    private async ValueTask triggerValidation(string arg)
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        var token = _cancellationTokenSource.Token;

        try
        {
            await Task.Delay(500, token);
            if (!token.IsCancellationRequested)
            {
                generateClass();
            }
        }
        catch (TaskCanceledException) { }
    }

    public void OnDataReceived(string dataTypeName, object? parsedData)
    {
        _input.Text(prettifyAsJsonOrDoNothing(parsedData.ToString()));
    }

    private async void onInputFileSelected(SandboxedFileReader[] files)
    {
        await onFileSelected(_input, files);
    }

    #endregion


    #region UIElements

    private readonly IUIMultiLineTextInput _input;
    private readonly IUIMultiLineTextInput _output;

    private readonly IUIFileSelector _inputFile;

    private readonly IUIStack _errorsStack = Stack().Vertical();

    private readonly IUIInfoBar _defaultError;

    #endregion

    #region methods
    private void generateClass()
    {
        var schema = validateAndParseSchema();
        if (schema == null)
        {
            return;
        }

        switch (_settingsProvider.GetSetting(_classGenerationType))
        {
            case ClassGenerationType.CSharp:
                var csharpGenerator = new NJsonSchema.CodeGeneration.CSharp.CSharpGenerator(schema);
                _output.Text(csharpGenerator.GenerateFile());
                break;
            case ClassGenerationType.TypeScript:
                var typeScriptGenerator =
                    new NJsonSchema.CodeGeneration.TypeScript.TypeScriptGenerator(schema);
                _output.Text(typeScriptGenerator.GenerateFile());
                break;
        }
    }

    private string setOutputLanguage()
    {
        return _settingsProvider.GetSetting(_classGenerationType) switch
        {
            ClassGenerationType.CSharp => "csharp",
            ClassGenerationType.TypeScript => "typescript",
            _ => "typescript"
        };
    }

    /**
         Prettify the JSON string if it is valid JSON, otherwise return the string as is.
    */
    private string prettifyAsJsonOrDoNothing(string str)
    {
        try
        {
            var prettified = JsonNode.Parse(str);
            return prettified.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
        }
        catch (JsonException)
        {
            return str;
        }
    }

    /**
        Get the error message if the JSON is invalid, otherwise return an empty string.
    */
    private string getJsonParseError(string json)
    {
        try
        {
            JsonNode.Parse(json);
            return string.Empty;
        }
        catch (JsonException e)
        {
            return e.Message;
        }
    }

    private JsonSchema? validateAndParseSchema()
    {
        if (string.IsNullOrWhiteSpace(_input.Text))
        {
            _errorsStack.WithChildren([_defaultError]);
        }
        else
        {
            var jsonError = getJsonParseError(_input.Text);
            if (!string.IsNullOrEmpty(jsonError))
            {
                _errorsStack.WithChildren(
                    getErrorInfoBar(JSONSchemaGenerator.InputError, jsonError)
                );
            }
            else
            {
                JsonSchema? schema = null;
                try
                {
                    schema = JsonSchema.FromJsonAsync(_input.Text).Result;
                }
                catch (Exception e)
                {
                    _errorsStack.WithChildren(
                        getErrorInfoBar(JSONSchemaClassGenerator.SchemaError, e.Message)
                    );
                }
                _errorsStack.WithChildren(
                    [
                        InfoBar()
                            .ShowIcon()
                            .Title(JSONSchemaGenerator.Success)
                            .Description(JSONSchemaGenerator.JsonSchemaGenerated)
                            .Success()
                            .Open()
                    ]
                );
                return schema;
            }
        }

        return null;
    }

    private IUIInfoBar getGeneralErrorInfoBar(string error)
    {
        return getErrorInfoBar(JSONSchemaGenerator.GeneralError, error);
    }

    private IUIInfoBar getErrorInfoBar(string title, string error)
    {
        return InfoBar().ShowIcon().Title(title).Description(error).NonClosable().Error().Open();
    }

    #endregion

    public UIToolView View
    {
        get
        {
            setOutputLanguage();
            _view ??= new UIToolView(
                Grid()
                    .Rows(
                        (GridRows.ConfigRow, Auto),
                        (GridRows.UploadRow, Auto),
                        (GridRows.InputRow, new UIGridLength(1, UIGridUnitType.Fraction)),
                        (GridRows.ErrorsRow, Auto)
                    )
                    .Columns((GridColumns.Stretch, new UIGridLength(1, UIGridUnitType.Fraction)))
                    .Cells(
                        Cell(
                            GridRows.ConfigRow,
                            GridColumns.Stretch,
                            Setting()
                                .Title(JSONSchemaClassGenerator.OutputType)
                                .Description(JSONSchemaClassGenerator.OutputTypeDescription)
                                .Handle(
                                    _settingsProvider,
                                    _classGenerationType,
                                    onOutputTypeChanged,
                                    Item("CSharp", ClassGenerationType.CSharp),
                                    Item("TypeScript", ClassGenerationType.TypeScript)
                                )
                        ),
                        Cell(GridRows.UploadRow, GridColumns.Stretch, _inputFile),
                        Cell(
                            GridRows.InputRow,
                            GridColumns.Stretch,
                            SplitGrid()
                                .Vertical()
                                .WithLeftPaneChild(_input)
                                .WithRightPaneChild(_output)
                        ),
                        Cell(
                            GridRows.ErrorsRow,
                            GridColumns.Stretch,
                            _errorsStack.WithChildren([_defaultError])
                        )
                    )
            );
            return _view;
        }
    }
}
