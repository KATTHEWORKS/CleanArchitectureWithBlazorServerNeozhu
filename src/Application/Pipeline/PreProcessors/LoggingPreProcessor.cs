// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Pipeline.PreProcessors;

public class LoggingPreProcessor<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger _logger;


    public LoggingPreProcessor(ILogger<TRequest> logger, ICurrentUserService currentUserService)
    {
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public Task Process(TRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var requestName = nameof(TRequest);
            var userName = _currentUserService.UserName;
            _logger.LogTrace("Request: {Name} with {@Request} by {@UserName}",
                requestName, request, userName);
            return Task.CompletedTask;
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