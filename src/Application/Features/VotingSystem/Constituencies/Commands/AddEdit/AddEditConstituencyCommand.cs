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
    public string? State {get;set;} 
    
    [Description("Name")]
    public string Name {get;set;} = String.Empty;

    [Description("Description")]
    public string? Description { get; set; }


    [Description("Mp Name Existing")]
    public string? MpNameExisting { get; set; }
    [Description("Existing Mp Party")]
    public string? ExistingMpParty { get; set; }

    [Description("Existing Mp Terms")]
    public string? ExistingMpTerms { get; set; }



    [Description("Other Past Mps")]
    public string? OtherPastMps {get;set; }  //name+party+terms

    //[Description("Read Count")]
    //public int ReadCount {get;set; } //how many users looking for this,can increase by 1 each time on cache & write once after certain time


    //[Description("Write Count")]
    //public int WriteCount {get;set;} 


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

