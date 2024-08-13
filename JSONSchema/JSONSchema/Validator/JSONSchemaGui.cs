using System.ComponentModel.Composition;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using CommunityToolkit.Diagnostics;
using DevToys.Api;
using NJsonSchema;
using NJsonSchema.Validation;
using static DevToys.Api.GUI;

namespace JSONSchema;

[Export(typeof(IGuiTool))]
[Name("BenTools.JSONSchema")] // A unique, internal name of the tool.
[ToolDisplayInformation(
    IconFontName = "DevToys-Tools-Icons", // This font is available by default in DevToys
    IconGlyph = '\u0108',
    GroupName = PredefinedCommonToolGroupNames.Testers, // The group in which the tool will appear in the side bar.
    ResourceManagerAssemblyIdentifier = nameof(ResourceAssemblyIdentifier), // The Resource Assembly Identifier to use
    ResourceManagerBaseName = "JSONSchema.Validator.JSONSchema", // The full name (including namespace) of the resource file containing our localized texts
    ShortDisplayTitleResourceName = nameof(JSONSchema.ShortDisplayTitle), // The name of the resource to use for the short display title
    LongDisplayTitleResourceName = nameof(JSONSchema.LongDisplayTitle),
    DescriptionResourceName = nameof(JSONSchema.Description),
    AccessibleNameResourceName = nameof(JSONSchema.AccessibleName)
)]
[AcceptedDataTypeName(PredefinedCommonDataTypeNames.Json)]
internal sealed class JSONSchemaGui : IGuiTool
{
    private UIToolView? _view;

    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    public JSONSchemaGui()
    {
        _input = MultiLineTextInput()
            .Title(JSONSchema.Input)
            .Language("json")
            .OnTextChanged(triggerValidation);

        _schema = MultiLineTextInput()
            .Title(JSONSchema.Schema)
            .Language("json")
            .OnTextChanged(triggerValidation);

        _inputFile = FileSelector("input-file")
            .CanSelectOneFile()
            .OnFilesSelected(onInputFileSelected)
            .LimitFileTypesTo(".json");

        _schemaFile = FileSelector("schema-file")
            .CanSelectOneFile()
            .OnFilesSelected(onSchemaFileSelected)
            .LimitFileTypesTo(".json");

        _defaultError = getGeneralErrorInfoBar(JSONSchema.JsonRequiredError);
    }

    #region enums

    enum GridRows
    {
        ConfigRow,
        ErrorsRow
    }

    enum InnerRows
    {
        UploadRow,
        RawRow
    }

    private enum GridColumns
    {
        Stretch
    }

    #endregion

    #region UIElements

    private readonly IUIMultiLineTextInput _input;
    private readonly IUIMultiLineTextInput _schema;

    private readonly IUIFileSelector _inputFile;
    private readonly IUIFileSelector _schemaFile;

    private readonly IUIStack _errorsStack = Stack().Vertical();

    private readonly IUIInfoBar _defaultError;

    #endregion

    #region events

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
                validate();
            }
        }
        catch (TaskCanceledException) { }
    }

    public void OnDataReceived(string dataTypeName, object? parsedData)
    {
        var str = parsedData.ToString();
        try
        {
            JsonSchema.FromJsonAsync(str);
            _schema.Text(prettifyAsJsonOrDoNothing(str));
        }
        catch (Exception)
        {
            _input.Text(prettifyAsJsonOrDoNothing(str));
        }
    }

    private async void onSchemaFileSelected(SandboxedFileReader[] files)
    {
        await onFileSelected(_schema, files);
    }

    private async void onInputFileSelected(SandboxedFileReader[] files)
    {
        await onFileSelected(_input, files);
    }

    private async ValueTask onFileSelected(IUIMultiLineTextInput input, SandboxedFileReader[] files)
    {
        Guard.HasSizeEqualTo(files, 1);
        using var memStream = new MemoryStream();
        using var file = files[0];

        await file.CopyFileContentToAsync(memStream, CancellationToken.None);
        var str = Encoding.UTF8.GetString(memStream.ToArray());
        input.Text(prettifyAsJsonOrDoNothing(str));
        validate();
    }


    #endregion

    #region methods
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

    /**
        If either the input or the schema is invalid JSON, return relevant error info bars.
    */
    private IUIInfoBar[] getJsonValidationErrorInfoBars()
    {
        IUIInfoBar[] errors = [];
        var jsonError = getJsonParseError(_input.Text);
        if (!string.IsNullOrEmpty(jsonError))
        {
            errors = errors.Append(getErrorInfoBar(JSONSchema.InputError, jsonError)).ToArray();
        }

        jsonError = getJsonParseError(_schema.Text);
        if (!string.IsNullOrEmpty(jsonError))
        {
            errors = errors.Append(getErrorInfoBar(JSONSchema.SchemaError, jsonError)).ToArray();
        }

        return errors;
    }

    private async void validate()
    {
        if (string.IsNullOrWhiteSpace(_input.Text) || string.IsNullOrWhiteSpace(_schema.Text))
        {
            _errorsStack.WithChildren([_defaultError]);
        }
        else
        {
            IUIInfoBar[] errors = getJsonValidationErrorInfoBars();
            if (!errors.Any())
            {
                try
                {
                    JsonSchema schema = await JsonSchema.FromJsonAsync(_schema.Text);
                    var schemaValidationErrors = schema.Validate(_input.Text);
                    if (schemaValidationErrors.Any())
                    {
                        foreach (var error in schemaValidationErrors)
                        {
                            errors = errors.Append(getSchemaValidationError(error)).ToArray();
                        }
                    }
                }
                catch (Exception e)
                {
                    errors = errors.Append(getGeneralErrorInfoBar(e.Message)).ToArray();
                }
            }

            if (!errors.Any())
            {
                _errorsStack.WithChildren(
                    [
                        InfoBar()
                            .ShowIcon()
                            .Title(JSONSchema.Success)
                            .Description(JSONSchema.JsonValid)
                            .Success()
                            .Open()
                    ]
                );
            }
            else
            {
                _errorsStack.WithChildren(errors);
            }
        }
    }

    private IUIInfoBar getGeneralErrorInfoBar(string error)
    {
        return getErrorInfoBar(JSONSchema.GeneralError, error);
    }

    private IUIInfoBar getSchemaValidationError(ValidationError error)
    {
        var errString = $"{error.Path}: {error.Kind}";
        if (error.HasLineInfo)
        {
            errString = string.Format(
                JSONSchema.SchemaValidationErrorWithLine,
                error.Path,
                error.LineNumber,
                error.LinePosition,
                error.Kind
            );
        }
        return getErrorInfoBar(JSONSchema.SchemaValidationError, errString);
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
            _view ??= new UIToolView(
                Grid()
                    .Rows(
                        (GridRows.ConfigRow, new UIGridLength(1, UIGridUnitType.Fraction)),
                        (GridRows.ErrorsRow, Auto)
                    )
                    .Columns((GridColumns.Stretch, new UIGridLength(1, UIGridUnitType.Fraction)))
                    .Cells(
                        Cell(
                            GridRows.ConfigRow,
                            GridColumns.Stretch,
                            SplitGrid()
                                .Vertical()
                                .WithLeftPaneChild(
                                    Grid()
                                        .Rows(
                                            (InnerRows.UploadRow, Auto),
                                            (
                                                InnerRows.RawRow,
                                                new UIGridLength(1, UIGridUnitType.Fraction)
                                            )
                                        )
                                        .Columns(
                                            (
                                                GridColumns.Stretch,
                                                new UIGridLength(1, UIGridUnitType.Fraction)
                                            )
                                        )
                                        .Cells(
                                            Cell(
                                                InnerRows.UploadRow,
                                                GridColumns.Stretch,
                                                _inputFile
                                            ),
                                            Cell(InnerRows.RawRow, GridColumns.Stretch, _input)
                                        )
                                )
                                .WithRightPaneChild(
                                    Grid()
                                        .Rows(
                                            (InnerRows.UploadRow, Auto),
                                            (
                                                InnerRows.RawRow,
                                                new UIGridLength(1, UIGridUnitType.Fraction)
                                            )
                                        )
                                        .Columns(
                                            (
                                                GridColumns.Stretch,
                                                new UIGridLength(1, UIGridUnitType.Fraction)
                                            )
                                        )
                                        .Cells(
                                            Cell(
                                                InnerRows.UploadRow,
                                                GridColumns.Stretch,
                                                _schemaFile
                                            ),
                                            Cell(InnerRows.RawRow, GridColumns.Stretch, _schema)
                                        )
                                )
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
