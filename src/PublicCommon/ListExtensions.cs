using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicCommon;
public static class ListExtensions
    {
    public static bool HasData<T>(this IEnumerable<T>? list)
        {
        return list != null && list.Any();
        }
    public static bool IsEmpty<T>(this IEnumerable<T>? list)
        {
        return list == null || !list.Any();
        }

    public static List<T> MoveItemToTopById<T>(this List<T> list, T? item, Func<T, int> getId)
        {
        if (item == null) return list;

        if (list == null)
            return [item];

        int targetId = getId(item);


        // Remove existing occurrences of the item (if any)
        if (list.RemoveAll(x => getId(x) == targetId) > 0)
            {
            // Insert the item at the top
            list.Insert(0, item);
            }
        return list;
        }

    }
