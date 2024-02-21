// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.Constituencies.Commands.Import;

public class ImportConstituenciesCommandValidator : AbstractValidator<ImportConstituenciesCommand>
{
        public ImportConstituenciesCommandValidator()
        {
           
           RuleFor(v => v.Data)
                .NotNull()
                .NotEmpty();

        }
}

