// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.TownProfiles.Commands.Import;

public class ImportTownProfilesCommandValidator : AbstractValidator<ImportTownProfilesCommand>
{
        public ImportTownProfilesCommandValidator()
        {
           
           RuleFor(v => v.Data)
                .NotNull()
                .NotEmpty();

        }
}

