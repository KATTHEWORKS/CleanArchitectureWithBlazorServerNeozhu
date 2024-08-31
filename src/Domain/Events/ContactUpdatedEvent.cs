﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Domain.Events;


    public class ContactUpdatedEvent : DomainEvent
    {
        public ContactUpdatedEvent(Contact item)
        {
            Item = item;
        }

        public Contact Item { get; }
    }
