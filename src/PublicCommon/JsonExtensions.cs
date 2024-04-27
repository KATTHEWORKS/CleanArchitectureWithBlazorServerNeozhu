using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PublicCommon;
public static class JsonExtensions
    {
    public static readonly JsonSerializerOptions IgnoreNullSerializationOptions = new JsonSerializerOptions
        {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

    public static T CloneBySerializing<T>(this T obj)
        {
        if (obj == null) return default(T);
        return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(obj));
        }

    public static bool TryDeserialize<T>(string? json, out T result, JsonSerializerOptions options = null)
        {
        try
            {
            if (string.IsNullOrEmpty(json))
                {
                result = default;
                return false;
                }
            result = JsonSerializer.Deserialize<T>(json, options == null ? IgnoreNullSerializationOptions : options);
            return true;
            }
        //catch (JsonException)
        //{
        //    result = default;
        //    return false;
        //}
        catch (Exception e)
            {
            System.Diagnostics.Trace.TraceError(e.ToString());
            Console.WriteLine(e.ToString());
            result = default;
            return false;
            }
        }
    //public static bool TryDeserialize<T>(string json, out T result)
    //{
    //    try
    //    {
    //        if (string.IsNullOrEmpty(json))
    //        {
    //            result = default;
    //            return false;
    //        }
    //        result = JsonSerializer.Deserialize<T>(json);
    //        return true;
    //    }
    //    //catch (JsonException)
    //    //{
    //    //    result = default;
    //    //    return false;
    //    //}
    //    catch (Exception e)
    //    {
    //        Console.WriteLine(e.ToString());
    //        result = default;
    //        return false;
    //    }
    //}
    }



