// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.Constituencies.EventHandlers;

    public class ConstituencyDeletedEventHandler : INotificationHandler<ConstituencyDeletedEvent>
    {
        private readonly ILogger<ConstituencyDeletedEventHandler> _logger;

        public ConstituencyDeletedEventHandler(
            ILogger<ConstituencyDeletedEventHandler> logger
            )
        {
            _logger = logger;
        }
        public Task Handle(ConstituencyDeletedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().FullName);
            return Task.CompletedTask;
        }
    }
