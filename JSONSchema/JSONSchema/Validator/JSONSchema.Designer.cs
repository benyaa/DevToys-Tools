namespace JSONSchema;
using System;

internal class JSONSchema
{

    private static global::System.Resources.ResourceManager resourceMan;

    private static global::System.Globalization.CultureInfo resourceCulture;

    [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    internal JSONSchema()
    {
    }

    /// <summary>
    ///   Returns the cached ResourceManager instance used by this class.
    /// </summary>
    [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
    internal static global::System.Resources.ResourceManager ResourceManager
    {
        get
        {
            if (object.ReferenceEquals(resourceMan, null))
            {
                global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("JSONSchema.Validator.JSONSchema", typeof(JSONSchema).Assembly);
                resourceMan = temp;
            }
            return resourceMan;
        }
    }

    /// <summary>
    ///   Overrides the current thread's CurrentUICulture property for all
    ///   resource lookups using this strongly typed resource class.
    /// </summary>
    [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
    internal static global::System.Globalization.CultureInfo Culture
    {
        get
        {
            return resourceCulture;
        }
        set
        {
            resourceCulture = value;
        }
    }

    /// <summary>
    ///   Looks up a localized string similar to A &quot;Hello World&quot; extension for DevToys.
    /// </summary>
    internal static string AccessibleName
    {
        get
        {
            return ResourceManager.GetString("AccessibleName", resourceCulture);
        }
    }

    /// <summary>
    ///   Looks up a localized string similar to A sample extension.
    /// </summary>
    internal static string Description
    {
        get
        {
            return ResourceManager.GetString("Description", resourceCulture);
        }
    }

    /// <summary>
    ///   Looks up a localized string similar to Hello World!.
    /// </summary>
    internal static string HelloWorldLabel
    {
        get
        {
            return ResourceManager.GetString("HelloWorldLabel", resourceCulture);
        }
    }

    /// <summary>
    ///   Looks up a localized string similar to My Awesome Extension.
    /// </summary>
    internal static string LongDisplayTitle
    {
        get
        {
            return ResourceManager.GetString("LongDisplayTitle", resourceCulture);
        }
    }

    /// <summary>
    ///   Looks up a localized string similar to My Extension.
    /// </summary>
    internal static string ShortDisplayTitle
    {
        get
        {
            return ResourceManager.GetString("ShortDisplayTitle", resourceCulture);
        }
    }

    internal static string Input
    {
        get
        {
            return ResourceManager.GetString("Input", resourceCulture);
        }
    }

    internal static string Schema
    {
        get
        {
            return ResourceManager.GetString("Schema", resourceCulture);
        }
    }


    internal static string GeneralError
    {
        get
        {
            return ResourceManager.GetString("GeneralError", resourceCulture);
        }
    }

    internal static string JsonRequiredError
    {
        get
        {
            return ResourceManager.GetString("JsonRequiredError", resourceCulture);
        }
    }

    internal static string InputError
    {
        get
        {
            return ResourceManager.GetString("InputError", resourceCulture);
        }
    }

    internal static string SchemaError
    {
        get
        {
            return ResourceManager.GetString("SchemaError", resourceCulture);
        }
    }

    internal static string JsonValid
    {
        get
        {
            return ResourceManager.GetString("JsonValid", resourceCulture);
        }
    }

    internal static string Success
    {
        get
        {
            return ResourceManager.GetString("Success", resourceCulture);
        }
    }

    internal static string SchemaValidationError
    {
        get
        {
            return ResourceManager.GetString("SchemaValidationError", resourceCulture);
        }
    }

    internal static string SchemaValidationErrorWithLine
    {
        get
        {
            return ResourceManager.GetString("SchemaValidationErrorWithLine", resourceCulture);
        }
    }
    
}