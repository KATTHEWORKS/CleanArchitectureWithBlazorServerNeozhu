// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.Commands.Update;

public class UpdateVoteCommandValidator : AbstractValidator<UpdateVoteCommand>
{
        public UpdateVoteCommandValidator()
        {
           RuleFor(v => v.Id).NotNull();
           //RuleFor(v => v.Name).MaximumLength(256).NotEmpty();
          
        }
    
}

