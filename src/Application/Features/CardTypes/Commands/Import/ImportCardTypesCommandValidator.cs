// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.CardTypes.Commands.Import;

public class ImportCardTypesCommandValidator : AbstractValidator<ImportCardTypesCommand>
{
        public ImportCardTypesCommandValidator()
        {
           
           RuleFor(v => v.Data)
                .NotNull()
                .NotEmpty();

        }
}

