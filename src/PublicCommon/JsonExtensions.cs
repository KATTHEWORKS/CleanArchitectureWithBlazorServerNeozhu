using System.Text.Json;

namespace PublicCommon;
public static class JsonExtensions
    {

    public static T CloneBySerializing<T>(this T obj)
        {
        try
            {
            if (obj != null)
                {
                return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(obj, CONSTANTS.DefaultSerializationJsonOptions));
                }
            }
        catch (Exception ex)
            {
            Console.WriteLine($"Clone failed for type {typeof(T)} with exception {ex.ToString()}");
            }
        return default(T);
        }

    public static string? Serialize<T>(this T obj)
        {
        if (obj != null)
            {
            return JsonSerializer.Serialize(obj, CONSTANTS.DefaultSerializationJsonOptions);
            }
        else return null;
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
            result = JsonSerializer.Deserialize<T>(json, options == null ? CONSTANTS.DefaultSerializationJsonOptions : options);
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



