using System;
using System.Linq.Expressions;
namespace CleanArchitecture.Blazor.Application.Common.Extensions;
public static class ExpressionExtensions
{
    public static Expression<Func<T, bool>> AndAlso<T>(
        this Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right)
    {
        var param = Expression.Parameter(typeof(T));
        var combined = Expression.AndAlso(
            Expression.Invoke(left, param),
            Expression.Invoke(right, param)
        );

        return Expression.Lambda<Func<T, bool>>(combined, param);
    }
}
