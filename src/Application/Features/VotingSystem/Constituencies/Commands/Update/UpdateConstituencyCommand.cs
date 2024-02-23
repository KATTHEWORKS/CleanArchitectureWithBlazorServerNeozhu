// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using System.ComponentModel;
using CleanArchitecture.Blazor.Application.Features.VotingSystem.Constituencies.DTOs;
using CleanArchitecture.Blazor.Application.Features.VotingSystem.Constituencies.Caching;

namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.Constituencies.Commands.Update;

public class UpdateConstituencyCommand: ICacheInvalidatorRequest<Result<int>>
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
            CreateMap<ConstituencyDto,UpdateConstituencyCommand>(MemberList.None);
            CreateMap<UpdateConstituencyCommand,VoteConstituency>(MemberList.None);
        }
    }
}

    public class UpdateConstituencyCommandHandler : IRequestHandler<UpdateConstituencyCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<UpdateConstituencyCommandHandler> _localizer;
        public UpdateConstituencyCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<UpdateConstituencyCommandHandler> localizer,
             IMapper mapper
            )
        {
            _context = context;
            _localizer = localizer;
            _mapper = mapper;
        }
        public async Task<Result<int>> Handle(UpdateConstituencyCommand request, CancellationToken cancellationToken)
        {

           var item =await _context.VoteConstituencies.FindAsync( new object[] { request.Id }, cancellationToken)?? throw new NotFoundException($"Constituency with id: [{request.Id}] not found.");
           item = _mapper.Map(request, item);
		    // raise a update domain event
		   item.AddDomainEvent(new ConstituencyUpdatedEvent(item));
           await _context.SaveChangesAsync(cancellationToken);
           return await Result<int>.SuccessAsync(item.Id);
        }
    }

