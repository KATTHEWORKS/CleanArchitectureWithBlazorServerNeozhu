// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.TownProfiles.EventHandlers;

    public class TownProfileDeletedEventHandler : INotificationHandler<TownProfileDeletedEvent>
    {
        private readonly ILogger<TownProfileDeletedEventHandler> _logger;

        public TownProfileDeletedEventHandler(
            ILogger<TownProfileDeletedEventHandler> logger
            )
        {
            _logger = logger;
        }
        public Task Handle(TownProfileDeletedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().FullName);
            return Task.CompletedTask;
        }
    }
