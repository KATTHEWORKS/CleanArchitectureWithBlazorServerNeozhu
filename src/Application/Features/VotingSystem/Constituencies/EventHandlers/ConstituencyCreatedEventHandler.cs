// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Events.VotingSystem;

namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.Constituencies.EventHandlers;

public class ConstituencyCreatedEventHandler : INotificationHandler<ConstituencyCreatedEvent>
{
        private readonly ILogger<ConstituencyCreatedEventHandler> _logger;

        public ConstituencyCreatedEventHandler(
            ILogger<ConstituencyCreatedEventHandler> logger
            )
        {
            _logger = logger;
        }
        public Task Handle(ConstituencyCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().FullName);
            return Task.CompletedTask;
        }
}
