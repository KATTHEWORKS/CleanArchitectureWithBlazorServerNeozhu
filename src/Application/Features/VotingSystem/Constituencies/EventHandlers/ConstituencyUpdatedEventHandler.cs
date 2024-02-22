// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Events.VotingSystem;

namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.Constituencies.EventHandlers;

public class ConstituencyUpdatedEventHandler : INotificationHandler<ConstituencyUpdatedEvent>
    {
        private readonly ILogger<ConstituencyUpdatedEventHandler> _logger;

        public ConstituencyUpdatedEventHandler(
            ILogger<ConstituencyUpdatedEventHandler> logger
            )
        {
            _logger = logger;
        }
        public Task Handle(ConstituencyUpdatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().FullName);
            return Task.CompletedTask;
        }
    }
