// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.CardTypes.EventHandlers;

public class CardTypeCreatedEventHandler : INotificationHandler<CardTypeCreatedEvent>
{
        private readonly ILogger<CardTypeCreatedEventHandler> _logger;

        public CardTypeCreatedEventHandler(
            ILogger<CardTypeCreatedEventHandler> logger
            )
        {
            _logger = logger;
        }
        public Task Handle(CardTypeCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().FullName);
            return Task.CompletedTask;
        }
}
