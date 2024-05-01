// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Towns.EventHandlers;

    public class TownDeletedEventHandler : INotificationHandler<TownDeletedEvent>
    {
        private readonly ILogger<TownDeletedEventHandler> _logger;

        public TownDeletedEventHandler(
            ILogger<TownDeletedEventHandler> logger
            )
        {
            _logger = logger;
        }
        public Task Handle(TownDeletedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().FullName);
            return Task.CompletedTask;
        }
    }
