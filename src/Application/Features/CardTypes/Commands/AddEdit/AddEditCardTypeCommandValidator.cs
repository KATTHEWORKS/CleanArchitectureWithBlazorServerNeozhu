// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.CardTypes.Commands.AddEdit;

public class AddEditCardTypeCommandValidator : AbstractValidator<AddEditCardTypeCommand>
{
    public AddEditCardTypeCommandValidator()
    {
            RuleFor(v => v.Name)
                .MaximumLength(256)
                .NotEmpty();
       
     }

}

