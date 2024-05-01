// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Towns.EventHandlers;

public class TownCreatedEventHandler : INotificationHandler<TownCreatedEvent>
{
        private readonly ILogger<TownCreatedEventHandler> _logger;

        public TownCreatedEventHandler(
            ILogger<TownCreatedEventHandler> logger
            )
        {
            _logger = logger;
        }
        public Task Handle(TownCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().FullName);
            return Task.CompletedTask;
        }
}
