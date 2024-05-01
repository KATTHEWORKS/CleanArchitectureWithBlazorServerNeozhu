// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.TypeOfProfileMasterDatas.EventHandlers;

public class TypeOfProfileMasterDataCreatedEventHandler : INotificationHandler<TypeOfProfileMasterDataCreatedEvent>
{
        private readonly ILogger<TypeOfProfileMasterDataCreatedEventHandler> _logger;

        public TypeOfProfileMasterDataCreatedEventHandler(
            ILogger<TypeOfProfileMasterDataCreatedEventHandler> logger
            )
        {
            _logger = logger;
        }
        public Task Handle(TypeOfProfileMasterDataCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().FullName);
            return Task.CompletedTask;
        }
}
