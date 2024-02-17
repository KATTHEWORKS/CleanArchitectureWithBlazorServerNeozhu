// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Common.Interfaces;

namespace CleanArchitecture.Blazor.Application.Pipeline;

public class MemoryCacheBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheableRequest<TResponse>
{
    private readonly IAppCache _cache;
    private readonly ILogger<MemoryCacheBehaviour<TRequest, TResponse>> _logger;

    public MemoryCacheBehaviour(
        IAppCache cache,
        ILogger<MemoryCacheBehaviour<TRequest, TResponse>> logger
    )
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogTrace("{Name} is caching with {@Request}", nameof(request), request);
            var response = await _cache.GetOrAddAsync(
                request.CacheKey,
                async () =>
                    await next(),
                request.Options).ConfigureAwait(false);

            return response;
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;
            var message = $" {requestName}: {ex.Message} with {request} ";
            Console.WriteLine(ex.ToString() + message);
            _logger.LogError(ex, message);
            //_logger.LogError(ex, "{Name}: {Exception} with {@Request} by {@UserName}", requestName, ex.Message, request,
            //  userName ?? "");
            throw;
        }
    }
}