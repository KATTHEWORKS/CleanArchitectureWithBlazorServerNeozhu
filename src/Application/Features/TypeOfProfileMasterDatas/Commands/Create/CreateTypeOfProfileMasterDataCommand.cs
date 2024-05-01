// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using System.ComponentModel;
using CleanArchitecture.Blazor.Application.Features.TypeOfProfileMasterDatas.DTOs;
using CleanArchitecture.Blazor.Application.Features.TypeOfProfileMasterDatas.Caching;

namespace CleanArchitecture.Blazor.Application.Features.TypeOfProfileMasterDatas.Commands.Create;

public class CreateTypeOfProfileMasterDataCommand: ICacheInvalidatorRequest<Result<int>>
{
      [Description("Id")]
      public int Id { get; set; }
          [Description("System Type Id")]
    public int SystemTypeId {get;set;} 
    [Description("Name")]
    public string Name {get;set;} = String.Empty; 
    [Description("Short Name")]
    public string? ShortName {get;set;} 
    [Description("Brief Description")]
    public string? BriefDescription {get;set;} 
    [Description("Price")]
    public int Price {get;set;} 

      public string CacheKey => TypeOfProfileMasterDataCacheKey.GetAllCacheKey;
      public CancellationTokenSource? SharedExpiryTokenSource => TypeOfProfileMasterDataCacheKey.SharedExpiryTokenSource();
    private class Mapping : Profile
    {
        public Mapping()
        {
             CreateMap<TypeOfProfileMasterDataDto,CreateTypeOfProfileMasterDataCommand>(MemberList.None);
             CreateMap<CreateTypeOfProfileMasterDataCommand,TypeOfProfileMasterData>(MemberList.None);
        }
    }
}
    
    public class CreateTypeOfProfileMasterDataCommandHandler : IRequestHandler<CreateTypeOfProfileMasterDataCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<CreateTypeOfProfileMasterDataCommand> _localizer;
        public CreateTypeOfProfileMasterDataCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<CreateTypeOfProfileMasterDataCommand> localizer,
            IMapper mapper
            )
        {
            _context = context;
            _localizer = localizer;
            _mapper = mapper;
        }
        public async Task<Result<int>> Handle(CreateTypeOfProfileMasterDataCommand request, CancellationToken cancellationToken)
        {
           var item = _mapper.Map<TypeOfProfileMasterData>(request);
           // raise a create domain event
	       item.AddDomainEvent(new TypeOfProfileMasterDataCreatedEvent(item));
           _context.TypeOfProfileMasterDatas.Add(item);
           await _context.SaveChangesAsync(cancellationToken);
           return  await Result<int>.SuccessAsync(item.Id);
        }
    }

