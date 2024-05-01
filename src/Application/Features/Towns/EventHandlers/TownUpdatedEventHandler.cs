// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Towns.EventHandlers;

    public class TownUpdatedEventHandler : INotificationHandler<TownUpdatedEvent>
    {
        private readonly ILogger<TownUpdatedEventHandler> _logger;

        public TownUpdatedEventHandler(
            ILogger<TownUpdatedEventHandler> logger
            )
        {
            _logger = logger;
        }
        public Task Handle(TownUpdatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().FullName);
            return Task.CompletedTask;
        }
    }
