// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using System.ComponentModel;
using CleanArchitecture.Blazor.Application.Features.CardTypes.DTOs;
using CleanArchitecture.Blazor.Application.Features.CardTypes.Caching;

namespace CleanArchitecture.Blazor.Application.Features.CardTypes.Commands.Create;

public class CreateCardTypeCommand: ICacheInvalidatorRequest<Result<int>>
{
      [Description("Id")]
      public int Id { get; set; }
          [Description("Name")]
    public string Name {get;set;} = String.Empty; 
    [Description("Short Name")]
    public string? ShortName {get;set;} 
    [Description("Description")]
    public string? Description {get;set;} 
    [Description("Price")]
    public int Price {get;set;} 
    [Description("Priority Order")]
    public byte? PriorityOrder {get;set;} 
    [Description("Required Approval Count")]
    public byte? RequiredApprovalCount {get;set;} 

      public string CacheKey => CardTypeCacheKey.GetAllCacheKey;
      public CancellationTokenSource? SharedExpiryTokenSource => CardTypeCacheKey.GetOrCreateTokenSource();
    private class Mapping : Profile
    {
        public Mapping()
        {
             CreateMap<CardTypeDto,CreateCardTypeCommand>(MemberList.None);
             CreateMap<CreateCardTypeCommand,CardType>(MemberList.None);
        }
    }
}
    
    public class CreateCardTypeCommandHandler : IRequestHandler<CreateCardTypeCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<CreateCardTypeCommand> _localizer;
        public CreateCardTypeCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<CreateCardTypeCommand> localizer,
            IMapper mapper
            )
        {
            _context = context;
            _localizer = localizer;
            _mapper = mapper;
        }
        public async Task<Result<int>> Handle(CreateCardTypeCommand request, CancellationToken cancellationToken)
        {
           var item = _mapper.Map<CardType>(request);
           // raise a create domain event
	       item.AddDomainEvent(new CardTypeCreatedEvent(item));
           _context.CardTypes.Add(item);
           await _context.SaveChangesAsync(cancellationToken);
           return  await Result<int>.SuccessAsync(item.Id);
        }
    }

