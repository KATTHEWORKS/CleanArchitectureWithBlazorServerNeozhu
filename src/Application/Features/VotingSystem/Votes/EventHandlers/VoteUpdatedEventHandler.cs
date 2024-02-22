// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Events.VotingSystem;

namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.EventHandlers;

public class VoteUpdatedEventHandler : INotificationHandler<VoteUpdatedEvent>
    {
        private readonly ILogger<VoteUpdatedEventHandler> _logger;

        public VoteUpdatedEventHandler(
            ILogger<VoteUpdatedEventHandler> logger
            )
        {
            _logger = logger;
        }
        public Task Handle(VoteUpdatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().FullName);
            return Task.CompletedTask;
        }
    }
