// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Events.VotingSystem;

namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.EventHandlers;

public class VoteDeletedEventHandler : INotificationHandler<VoteDeletedEvent>
    {
        private readonly ILogger<VoteDeletedEventHandler> _logger;

        public VoteDeletedEventHandler(
            ILogger<VoteDeletedEventHandler> logger
            )
        {
            _logger = logger;
        }
        public Task Handle(VoteDeletedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().FullName);
            return Task.CompletedTask;
        }
    }
