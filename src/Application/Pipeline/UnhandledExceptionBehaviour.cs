// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Pipeline;

public class UnhandledExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<TRequest> _logger;

    public UnhandledExceptionBehaviour(ILogger<TRequest> logger, ICurrentUserService currentUserService)
    {
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;
            var userName = _currentUserService?.UserName;
            var message = $" {requestName}: {ex.Message} with {request} by {userName ?? "null-username"}";
            Console.WriteLine(ex.ToString() + message);
            _logger.LogError(ex, message);
            //_logger.LogError(ex, "{Name}: {Exception} with {@Request} by {@UserName}", requestName, ex.Message, request,
            //  userName ?? "");
            throw;
        }
    }
}