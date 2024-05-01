// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.TownProfiles.EventHandlers;

public class TownProfileCreatedEventHandler : INotificationHandler<TownProfileCreatedEvent>
{
        private readonly ILogger<TownProfileCreatedEventHandler> _logger;

        public TownProfileCreatedEventHandler(
            ILogger<TownProfileCreatedEventHandler> logger
            )
        {
            _logger = logger;
        }
        public Task Handle(TownProfileCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().FullName);
            return Task.CompletedTask;
        }
}
