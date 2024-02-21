// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.VoteSummaries.EventHandlers;

    public class VoteSummaryUpdatedEventHandler : INotificationHandler<VoteSummaryUpdatedEvent>
    {
        private readonly ILogger<VoteSummaryUpdatedEventHandler> _logger;

        public VoteSummaryUpdatedEventHandler(
            ILogger<VoteSummaryUpdatedEventHandler> logger
            )
        {
            _logger = logger;
        }
        public Task Handle(VoteSummaryUpdatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().FullName);
            return Task.CompletedTask;
        }
    }
