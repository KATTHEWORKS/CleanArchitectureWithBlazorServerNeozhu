// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Events.VotingSystem;

namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.VoteSummaries.EventHandlers;

public class VoteSummaryCreatedEventHandler : INotificationHandler<VoteSummaryCreatedEvent>
{
        private readonly ILogger<VoteSummaryCreatedEventHandler> _logger;

        public VoteSummaryCreatedEventHandler(
            ILogger<VoteSummaryCreatedEventHandler> logger
            )
        {
            _logger = logger;
        }
        public Task Handle(VoteSummaryCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().FullName);
            return Task.CompletedTask;
        }
}
