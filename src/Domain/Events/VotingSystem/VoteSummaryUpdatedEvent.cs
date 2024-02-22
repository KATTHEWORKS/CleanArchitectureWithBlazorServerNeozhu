// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Domain.Events.VotingSystem;


public class VoteSummaryUpdatedEvent : DomainEvent
{
    public VoteSummaryUpdatedEvent(VoteSummary item)
    {
        Item = item;
    }

    public VoteSummary Item { get; }
}

