// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.Constituencies.Commands.AddEdit;

public class AddEditConstituencyCommandValidator : AbstractValidator<AddEditConstituencyCommand>
{
    public AddEditConstituencyCommandValidator()
    {
            RuleFor(v => v.Name)
                .MaximumLength(256)
                .NotEmpty();
       
     }

}

