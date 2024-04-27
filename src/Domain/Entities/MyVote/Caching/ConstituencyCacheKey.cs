// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace Domain.MyVote.Caching;

public static class ConstituencyCacheKey
{
    private static readonly TimeSpan refreshInterval = TimeSpan.FromHours(3);
    public const string GetAllCacheKey = "all-Constituencies";
    public static string GetPaginationCacheKey(string parameters)
    {
        return $"ConstituencyCacheKey:ConstituenciesWithPaginationQuery,{parameters}";
    }
    public static string GetByNameCacheKey(string parameters)
    {
        return $"ConstituencyCacheKey:GetByNameCacheKey,{parameters}";
    }
    public static string GetByIdCacheKey(string parameters)
    {
        return $"ConstituencyCacheKey:GetByIdCacheKey,{parameters}";
    }
    static ConstituencyCacheKey()
    {
        _tokensource = new CancellationTokenSource(refreshInterval);
    }
    private static CancellationTokenSource _tokensource;
    public static CancellationTokenSource SharedExpiryTokenSource()
    {
        if (_tokensource.IsCancellationRequested)
        {
            _tokensource = new CancellationTokenSource(refreshInterval);
        }
        return _tokensource;
    }
    public static void Refresh() => SharedExpiryTokenSource().Cancel();
    public static MemoryCacheEntryOptions MemoryCacheEntryOptions => new MemoryCacheEntryOptions().AddExpirationToken(new CancellationChangeToken(SharedExpiryTokenSource().Token));
}

