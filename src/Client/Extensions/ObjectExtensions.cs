using System;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Web;
using System.Globalization;
using MudBlazor.Extensions;

namespace Client.Extensions;

public static class ObjectExtensions
{
    //public static string AsQueryString(this object obj)
    //{
    //    var properties = from p in obj.GetType().GetProperties()
    //                     where p.GetValue(obj, null) != null
    //                     select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(obj, null).ToString());

    //    return string.Join("&", properties.ToArray());
    //}
    

    // Modified code to make it work with enum array and datetime
    public static string AsQueryString(this object obj)
    {
        var properties = from p in obj.GetType().GetProperties()
                         let value = p.GetValue(obj, null)
                         where value != null
                         select GetQueryStringValue(p.Name, value);

        return string.Join("&", properties.ToArray());
    }


    private static string GetQueryStringValue(string propertyName, object value)
    {

        var type = value.GetType();

        if (type.IsArray && type.GetElementType().IsEnum)
        {
            // Handle array of enums
            var enumArray = (Array)value;

            var serializedEnums = enumArray.Cast<object>()
                .Select(enumValue => propertyName + "=" + JsonSerializer.Serialize(enumValue, new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } }));

            return HttpUtility.UrlEncode(string.Join("&", serializedEnums));
        }

        if (type.IsEnum)
        {
            return propertyName + "=" + HttpUtility.UrlEncode(JsonSerializer.Serialize(value, new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } }));
        }

        if (type == typeof(DateTime))
        {
            var dateTimeValue = (DateTime)value;
            return propertyName + "=" + HttpUtility.UrlEncode(dateTimeValue.ToIsoDateString());
        }


        return propertyName + "=" + HttpUtility.UrlEncode(value.ToString());
    }
}