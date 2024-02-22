// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using System.ComponentModel;
using CleanArchitecture.Blazor.Application.Features.VotingSystem.Constituencies.DTOs;
using CleanArchitecture.Blazor.Application.Features.VotingSystem.Constituencies.Caching;
using CleanArchitecture.Blazor.Domain.Entities.VotingSystem;

namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.Constituencies.Commands.Create;

public class CreateConstituencyCommand: ICacheInvalidatorRequest<Result<int>>
{
      [Description("Id")]
      public int Id { get; set; }
          [Description("State Name")]
    public string? State {get;set;} 
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
             CreateMap<ConstituencyDto,CreateConstituencyCommand>(MemberList.None);
             CreateMap<CreateConstituencyCommand,Constituency>(MemberList.None);
        }
    }
}
    
    public class CreateConstituencyCommandHandler : IRequestHandler<CreateConstituencyCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<CreateConstituencyCommand> _localizer;
        public CreateConstituencyCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<CreateConstituencyCommand> localizer,
            IMapper mapper
            )
        {
            _context = context;
            _localizer = localizer;
            _mapper = mapper;
        }
        public async Task<Result<int>> Handle(CreateConstituencyCommand request, CancellationToken cancellationToken)
        {
           var item = _mapper.Map<Constituency>(request);
           // raise a create domain event
	       item.AddDomainEvent(new ConstituencyCreatedEvent(item));
           _context.Constituencies.Add(item);
           await _context.SaveChangesAsync(cancellationToken);
           return  await Result<int>.SuccessAsync(item.Id);
        }
    }

