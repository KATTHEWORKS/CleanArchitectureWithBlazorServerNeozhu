using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace PublicCommon;
public static class EnumExtensions
    {
    //since enums are mostly int so maintaining int as input, if sbyte requried then overload & parse
    public static TEnum? ParseToEnum<TEnum>(this int value) where TEnum : struct, Enum
        {
        if (Enum.IsDefined(typeof(TEnum), value))
            {
            return (TEnum)Enum.ToObject(typeof(TEnum), value);
            }
        else
            {
            return null;
            }
        }
    public static TEnum? MaxEnumValue<TEnum>(this IEnumerable<string> enumStringValues) where TEnum : struct
        {
        if (!typeof(TEnum).IsEnum)
            {
            throw new ArgumentException($"{nameof(TEnum)} must be an enumerated type");
            }

        var enumValues = enumStringValues
            .Select(enumStr => Enum.TryParse<TEnum>(enumStr, out var enumValue) ? enumValue : (TEnum?)null)
            .Where(enumValue => enumValue.HasValue)
            .Select(enumValue => enumValue!.Value);

        return enumValues.Any() ? enumValues.Max() : null;
        }

    public static string MaxEnumString<TEnum>(this IEnumerable<string>? enumStringValues) where TEnum : struct
        {
        if (!typeof(TEnum).IsEnum)
            {
            throw new ArgumentException($"{nameof(TEnum)} must be an enumerated type");
            }
        if (enumStringValues == null || !enumStringValues.Any()) return string.Empty;

        var enumValues = enumStringValues
            .Where(enumStr => Enum.TryParse<TEnum>(enumStr, out _))
            .DefaultIfEmpty()
            .Max();

        return enumValues ?? "No valid enum values";
        }

    // var sortedList = EnumSorting.SortByEnum<string, MyEnum>(stringList);
    /* need to validate more
      public static List<CustomType> SortByEnum<CustomType, enumType>(IEnumerable<CustomType?> itemList, bool descending = false) where enumType : IComparable
            {
            itemList = itemList.Distinct().ToList();
            //below extracts all enum values
            var itemToEnumMap = Enum.GetValues(typeof(enumType))
                                     .Cast<enumType>()
                                     .ToDictionary(enumValue => enumValue.ToString());

            //then sort by value
            if (descending)
                return itemList.OrderByDescending(item => itemToEnumMap[item.ToString()]).ToList();
            return itemList.OrderBy(item => itemToEnumMap[item.ToString()]).ToList();
            }*/

    public static Enum? MaxEnum<TEnum>(this IEnumerable<Enum>? enumValues) where TEnum : struct
        {
        if (!typeof(TEnum).IsEnum)
            {
            throw new ArgumentException($"{nameof(TEnum)} must be an enumerated type");
            }
        if (enumValues == null || !enumValues.Any()) return null;

        return enumValues.OrderBy(e => Convert.ToByte(e)).Last();//ascending order
        }
    public static List<T> GetEnumValues<T>()
        {
        if (!typeof(T).IsEnum)
            {
            throw new ArgumentException("Type parameter T must be an enum type.");
            }

        Array enumArray = Enum.GetValues(typeof(T));
        List<T> enumValues = new List<T>(enumArray.Length);
        foreach (var value in enumArray)
            {
            enumValues.Add((T)value);
            }

        return enumValues;
        }

    public static string GetEnumDescription(this Enum enumValue)
        {
        var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
        if (fieldInfo != null)
            {
            var descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : enumValue.ToString();
            }
        return string.Empty;
        }

    /// <summary>
    /// ByDisplayAttribute
    /// </summary>
    /// <param name="enumValue"></param>
    /// <returns></returns>
    public static (string Name, string Description) GetEnumDescriptionByDisplayAttribute(this Enum enumValue)
        {
        var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
        if (fieldInfo != null)
            {
            DisplayAttribute? displayAttribute =
            (DisplayAttribute)fieldInfo.GetCustomAttribute(typeof(DisplayAttribute), false);

            string name = displayAttribute?.Name ?? enumValue.ToString();
            string description = displayAttribute?.Description ?? enumValue.ToString();

            return (name, description);
            }
        else return (string.Empty, string.Empty);
        //var args = fieldInfo.CustomAttributes.First().NamedArguments;
        //string name = args.Any(x => x.MemberName == "Name") ? args.Where(x => x.MemberName == "Name").First().TypedValue.Value.ToString() : enumValue.ToString();
        //string description = args.Any(x => x.MemberName == "Description") ? args.Where(x => x.MemberName == "Name").First().TypedValue.Value.ToString() : name;

        //return (name, description);
        }
    }
