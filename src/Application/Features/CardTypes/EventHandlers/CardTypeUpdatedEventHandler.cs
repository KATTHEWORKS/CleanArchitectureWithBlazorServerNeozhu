// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.CardTypes.EventHandlers;

    public class CardTypeUpdatedEventHandler : INotificationHandler<CardTypeUpdatedEvent>
    {
        private readonly ILogger<CardTypeUpdatedEventHandler> _logger;

        public CardTypeUpdatedEventHandler(
            ILogger<CardTypeUpdatedEventHandler> logger
            )
        {
            _logger = logger;
        }
        public Task Handle(CardTypeUpdatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().FullName);
            return Task.CompletedTask;
        }
    }
