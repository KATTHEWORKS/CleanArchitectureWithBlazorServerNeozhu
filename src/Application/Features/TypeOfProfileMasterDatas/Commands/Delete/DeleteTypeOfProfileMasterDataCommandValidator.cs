// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.TypeOfProfileMasterDatas.Commands.Delete;

public class DeleteTypeOfProfileMasterDataCommandValidator : AbstractValidator<DeleteTypeOfProfileMasterDataCommand>
{
        public DeleteTypeOfProfileMasterDataCommandValidator()
        {
          
            RuleFor(v => v.Id).NotNull().ForEach(v=>v.GreaterThan(0));
          
        }
}
    

