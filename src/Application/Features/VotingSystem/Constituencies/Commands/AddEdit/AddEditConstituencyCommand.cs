// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.VotingSystem.Constituencies.DTOs;
using CleanArchitecture.Blazor.Application.Features.VotingSystem.Constituencies.Caching;
namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.Constituencies.Commands.AddEdit;

public class AddEditConstituencyCommand: ICacheInvalidatorRequest<Result<int>>
{
      [Description("Id")]
      public int Id { get; set; }
          [Description("State Name")]
    public string? StateName {get;set;} 
    [Description("Name")]
    public string Name {get;set;} = String.Empty; 
    [Description("Existing Mp Name")]
    public string? ExistingMpName {get;set;} 
    [Description("Alternate Mp Names")]
    public string? AlternateMpNames {get;set;} 
    [Description("Description")]
    public string? Description {get;set;} 
    [Description("Read Count")]
    public int ReadCount {get;set;} 
    [Description("Write Count")]
    public int WriteCount {get;set;} 


      public string CacheKey => ConstituencyCacheKey.GetAllCacheKey;
      public CancellationTokenSource? SharedExpiryTokenSource => ConstituencyCacheKey.SharedExpiryTokenSource();

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ConstituencyDto,AddEditConstituencyCommand>(MemberList.None);
            CreateMap<AddEditConstituencyCommand,Constituency>(MemberList.None);
         
        }
    }
}

    public class AddEditConstituencyCommandHandler : IRequestHandler<AddEditConstituencyCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<AddEditConstituencyCommandHandler> _localizer;
        public AddEditConstituencyCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<AddEditConstituencyCommandHandler> localizer,
            IMapper mapper
            )
        {
            _context = context;
            _localizer = localizer;
            _mapper = mapper;
        }
        public async Task<Result<int>> Handle(AddEditConstituencyCommand request, CancellationToken cancellationToken)
        {
            if (request.Id > 0)
            {
                var item = await _context.Constituencies.FindAsync(new object[] { request.Id }, cancellationToken) ?? throw new NotFoundException($"Constituency with id: [{request.Id}] not found.");
                item = _mapper.Map(request, item);
				// raise a update domain event
				item.AddDomainEvent(new ConstituencyUpdatedEvent(item));
                await _context.SaveChangesAsync(cancellationToken);
                return await Result<int>.SuccessAsync(item.Id);
            }
            else
            {
                var item = _mapper.Map<Constituency>(request);
                // raise a create domain event
				item.AddDomainEvent(new ConstituencyCreatedEvent(item));
                _context.Constituencies.Add(item);
                await _context.SaveChangesAsync(cancellationToken);
                return await Result<int>.SuccessAsync(item.Id);
            }
           
        }
    }

