// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Events.VotingSystem;

namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.VoteSummaries.EventHandlers;

public class VoteSummaryDeletedEventHandler : INotificationHandler<VoteSummaryDeletedEvent>
    {
        private readonly ILogger<VoteSummaryDeletedEventHandler> _logger;

        public VoteSummaryDeletedEventHandler(
            ILogger<VoteSummaryDeletedEventHandler> logger
            )
        {
            _logger = logger;
        }
        public Task Handle(VoteSummaryDeletedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().FullName);
            return Task.CompletedTask;
        }
    }
