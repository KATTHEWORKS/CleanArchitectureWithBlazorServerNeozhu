// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.TownProfiles.EventHandlers;

    public class TownProfileUpdatedEventHandler : INotificationHandler<TownProfileUpdatedEvent>
    {
        private readonly ILogger<TownProfileUpdatedEventHandler> _logger;

        public TownProfileUpdatedEventHandler(
            ILogger<TownProfileUpdatedEventHandler> logger
            )
        {
            _logger = logger;
        }
        public Task Handle(TownProfileUpdatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().FullName);
            return Task.CompletedTask;
        }
    }
