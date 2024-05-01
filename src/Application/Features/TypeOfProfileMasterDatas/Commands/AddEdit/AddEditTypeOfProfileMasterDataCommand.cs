// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.TypeOfProfileMasterDatas.DTOs;
using CleanArchitecture.Blazor.Application.Features.TypeOfProfileMasterDatas.Caching;
namespace CleanArchitecture.Blazor.Application.Features.TypeOfProfileMasterDatas.Commands.AddEdit;

public class AddEditTypeOfProfileMasterDataCommand: ICacheInvalidatorRequest<Result<int>>
{
      [Description("Id")]
      public int Id { get; set; }
          [Description("System Type Id")]
    public int SystemTypeId {get;set;} 
    [Description("Name")]
    public string Name {get;set;} = String.Empty; 
    [Description("Short Name")]
    public string? ShortName {get;set;} 
    [Description("Description")]
    public string? Description {get;set;} 
    [Description("Price")]
    public int Price {get;set;} 


      public string CacheKey => TypeOfProfileMasterDataCacheKey.GetAllCacheKey;
      public CancellationTokenSource? SharedExpiryTokenSource => TypeOfProfileMasterDataCacheKey.SharedExpiryTokenSource();

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<TypeOfProfileMasterDataDto,AddEditTypeOfProfileMasterDataCommand>(MemberList.None);
            CreateMap<AddEditTypeOfProfileMasterDataCommand,TypeOfProfileMasterData>(MemberList.None);
         
        }
    }
}

    public class AddEditTypeOfProfileMasterDataCommandHandler : IRequestHandler<AddEditTypeOfProfileMasterDataCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<AddEditTypeOfProfileMasterDataCommandHandler> _localizer;
        public AddEditTypeOfProfileMasterDataCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<AddEditTypeOfProfileMasterDataCommandHandler> localizer,
            IMapper mapper
            )
        {
            _context = context;
            _localizer = localizer;
            _mapper = mapper;
        }
        public async Task<Result<int>> Handle(AddEditTypeOfProfileMasterDataCommand request, CancellationToken cancellationToken)
        {
            if (request.Id > 0)
            {
                var item = await _context.TypeOfProfileMasterDatas.FindAsync(new object[] { request.Id }, cancellationToken) ?? throw new NotFoundException($"TypeOfProfileMasterData with id: [{request.Id}] not found.");
                item = _mapper.Map(request, item);
				// raise a update domain event
				item.AddDomainEvent(new TypeOfProfileMasterDataUpdatedEvent(item));
                await _context.SaveChangesAsync(cancellationToken);
                return await Result<int>.SuccessAsync(item.Id);
            }
            else
            {
                var item = _mapper.Map<TypeOfProfileMasterData>(request);
                // raise a create domain event
				item.AddDomainEvent(new TypeOfProfileMasterDataCreatedEvent(item));
                _context.TypeOfProfileMasterDatas.Add(item);
                await _context.SaveChangesAsync(cancellationToken);
                return await Result<int>.SuccessAsync(item.Id);
            }
           
        }
    }

