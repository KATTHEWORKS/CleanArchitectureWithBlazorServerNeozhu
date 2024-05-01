// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Towns.Commands.Create;

public class CreateTownCommandValidator : AbstractValidator<CreateTownCommand>
{
        public CreateTownCommandValidator()
        {
           
            RuleFor(v => v.Name)
                 .MaximumLength(256)
                 .NotEmpty();
        
        }
       
}

