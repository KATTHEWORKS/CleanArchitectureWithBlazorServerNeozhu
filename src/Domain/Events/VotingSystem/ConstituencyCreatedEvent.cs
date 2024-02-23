// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Entities.VotingSystem;

namespace CleanArchitecture.Blazor.Domain.Events.VotingSystem;

public class ConstituencyCreatedEvent : DomainEvent
{
    public ConstituencyCreatedEvent(VoteConstituency item)
    {
        Item = item;
    }

    public VoteConstituency Item { get; }
}

