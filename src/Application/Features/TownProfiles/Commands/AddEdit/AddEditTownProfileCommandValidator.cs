// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.TownProfiles.Commands.AddEdit;

public class AddEditTownProfileCommandValidator : AbstractValidator<AddEditTownProfileCommand>
{
    public AddEditTownProfileCommandValidator()
    {
            RuleFor(v => v.Name)
                .MaximumLength(256)
                .NotEmpty();
       
     }

}

