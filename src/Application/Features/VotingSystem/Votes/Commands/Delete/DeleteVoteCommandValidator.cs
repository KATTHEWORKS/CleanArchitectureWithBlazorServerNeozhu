// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.Commands.Delete;

public class DeleteVoteCommandValidator : AbstractValidator<DeleteVoteCommand>
{
        public DeleteVoteCommandValidator()
        {
          
            RuleFor(v => v.Id).NotNull().ForEach(v=>v.GreaterThan(0));
          
        }
}
    

