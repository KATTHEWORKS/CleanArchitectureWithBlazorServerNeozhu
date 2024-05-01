// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Towns.Commands.Import;

public class ImportTownsCommandValidator : AbstractValidator<ImportTownsCommand>
{
        public ImportTownsCommandValidator()
        {
           
           RuleFor(v => v.Data)
                .NotNull()
                .NotEmpty();

        }
}

