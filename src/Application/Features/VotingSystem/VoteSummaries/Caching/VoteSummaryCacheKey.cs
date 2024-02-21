// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.VoteSummaries.Caching;

public static class VoteSummaryCacheKey
{
    private static readonly TimeSpan refreshInterval = TimeSpan.FromHours(3);
    public const string GetAllCacheKey = "all-VoteSummaries";
    public static string GetPaginationCacheKey(string parameters) {
        return $"VoteSummaryCacheKey:VoteSummariesWithPaginationQuery,{parameters}";
    }
    public static string GetByNameCacheKey(string parameters) {
        return $"VoteSummaryCacheKey:GetByNameCacheKey,{parameters}";
    }
    public static string GetByIdCacheKey(string parameters) {
        return $"VoteSummaryCacheKey:GetByIdCacheKey,{parameters}";
    }
    static VoteSummaryCacheKey()
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

