// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.EventHandlers;

public class VoteCreatedEventHandler : INotificationHandler<VoteCreatedEvent>
{
        private readonly ILogger<VoteCreatedEventHandler> _logger;

        public VoteCreatedEventHandler(
            ILogger<VoteCreatedEventHandler> logger
            )
        {
            _logger = logger;
        }
        public Task Handle(VoteCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().FullName);
            return Task.CompletedTask;
        }
}
