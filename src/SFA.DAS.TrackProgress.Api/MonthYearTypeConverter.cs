using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SFA.DAS.TrackProgress.Api;

public static class MonthYearConverter
{
    public static DateOnly ToDateOnly(this object? value)
    {
        if (value is string str)
        {
            var cultureInfo = CultureInfo.InvariantCulture;
            if (DateOnly.TryParse(str, cultureInfo, DateTimeStyles.None, out var dateOnly))
                return dateOnly;

            if (DateTime.TryParse(str, cultureInfo, DateTimeStyles.None, out var dateTime))
                return dateTime.ToDateOnly();

            // also support MMYYY
        }

        throw new ArgumentException($"`{value}` cannot be converted to MMYYY", nameof(value));
    }
}
public class MonthYearTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string str) return str.ToDateOnly();
        return base.ConvertFrom(context, culture, value);
    }

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
        return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (destinationType == typeof(string) && value is DateOnly date) return date.ToString("O");
        return base.ConvertTo(context, culture, value, destinationType);
    }
}

public sealed class MonthYearJsonConverter : JsonConverter<DateOnly>
{
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString().ToDateOnly();
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        var isoDate = value.ToString("O");
        writer.WriteStringValue(isoDate);
    }
}

public static class MvcOptionsExtensions
{
    public static MvcOptions UseMonthYearTypeConverter(this MvcOptions options)
    {
        TypeDescriptor.AddAttributes(typeof(DateOnly), new TypeConverterAttribute(typeof(MonthYearTypeConverter)));
        return options;
    }

    public static JsonOptions UseMonthYearTypeConverter(this JsonOptions options)
    {
        options.JsonSerializerOptions.Converters.Add(new MonthYearJsonConverter());
        return options;
    }

    public static void UseMonthYearTypeConverter(this SwaggerGenOptions options)
    {
        options.MapType<DateOnly>(() => new OpenApiSchema
        {
            Type = "string",
            Format = "date",
            Example = OpenApiAnyFactory.CreateFromJson(@"""10-2020""")
        });
    }
}