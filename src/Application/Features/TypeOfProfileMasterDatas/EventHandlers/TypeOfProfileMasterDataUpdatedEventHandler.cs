// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.TypeOfProfileMasterDatas.EventHandlers;

    public class TypeOfProfileMasterDataUpdatedEventHandler : INotificationHandler<TypeOfProfileMasterDataUpdatedEvent>
    {
        private readonly ILogger<TypeOfProfileMasterDataUpdatedEventHandler> _logger;

        public TypeOfProfileMasterDataUpdatedEventHandler(
            ILogger<TypeOfProfileMasterDataUpdatedEventHandler> logger
            )
        {
            _logger = logger;
        }
        public Task Handle(TypeOfProfileMasterDataUpdatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().FullName);
            return Task.CompletedTask;
        }
    }
