// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.TypeOfProfileMasterDatas.Commands.Create;

public class CreateTypeOfProfileMasterDataCommandValidator : AbstractValidator<CreateTypeOfProfileMasterDataCommand>
{
        public CreateTypeOfProfileMasterDataCommandValidator()
        {
           
            RuleFor(v => v.Name)
                 .MaximumLength(256)
                 .NotEmpty();
        
        }
       
}

