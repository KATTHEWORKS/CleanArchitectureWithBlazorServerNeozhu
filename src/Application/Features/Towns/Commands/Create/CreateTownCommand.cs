// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using System.ComponentModel;
using CleanArchitecture.Blazor.Application.Features.Towns.DTOs;
using CleanArchitecture.Blazor.Application.Features.Towns.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Towns.Commands.Create;

public class CreateTownCommand: ICacheInvalidatorRequest<Result<int>>
{
      [Description("Id")]
      public int Id { get; set; }
          [Description("District")]
    public string? District {get;set;} 
    [Description("State")]
    public string? State {get;set;} 
    [Description("Url Name 1")]
    public string? UrlName1 {get;set;} 
    [Description("Url Name 2")]
    public string? UrlName2 {get;set;} 
    [Description("Active")]
    public bool Active {get;set;} 
    [Description("Name")]
    public string Name {get;set;} = String.Empty; 
    [Description("Sub Title")]
    public string? SubTitle {get;set;} 
    [Description("Description")]
    public string? Description {get;set;} 
    [Description("Image Url")]
    public string? ImageUrl {get;set;} 
    [Description("Address")]
    public string? Address {get;set;} 
    [Description("Mobile Number")]
    public string? MobileNumber {get;set;} 
    [Description("Google Map Address Url")]
    public string? GoogleMapAddressUrl {get;set;} 
    [Description("End Date To Show")]
    public DateTime EndDateToShow {get;set;} 
    [Description("Priotiry Order")]
    public int? PriotiryOrder {get;set;} 
    [Description("Google Profile Url")]
    public string? GoogleProfileUrl {get;set;} 
    [Description("Face Book Url")]
    public string? FaceBookUrl {get;set;} 
    [Description("You Tube Url")]
    public string? YouTubeUrl {get;set;} 
    [Description("Instagram Url")]
    public string? InstagramUrl {get;set;} 
    [Description("Twitter Url")]
    public string? TwitterUrl {get;set;} 
    [Description("Other Reference Url")]
    public string? OtherReferenceUrl {get;set;} 

      public string CacheKey => TownCacheKey.GetAllCacheKey;
      public CancellationTokenSource? SharedExpiryTokenSource => TownCacheKey.SharedExpiryTokenSource();
    private class Mapping : Profile
    {
        public Mapping()
        {
             CreateMap<TownDto,CreateTownCommand>(MemberList.None);
             CreateMap<CreateTownCommand,Town>(MemberList.None);
        }
    }
}
    
    public class CreateTownCommandHandler : IRequestHandler<CreateTownCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<CreateTownCommand> _localizer;
        public CreateTownCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<CreateTownCommand> localizer,
            IMapper mapper
            )
        {
            _context = context;
            _localizer = localizer;
            _mapper = mapper;
        }
        public async Task<Result<int>> Handle(CreateTownCommand request, CancellationToken cancellationToken)
        {
           var item = _mapper.Map<Town>(request);
           // raise a create domain event
	       item.AddDomainEvent(new TownCreatedEvent(item));
           _context.Towns.Add(item);
           await _context.SaveChangesAsync(cancellationToken);
           return  await Result<int>.SuccessAsync(item.Id);
        }
    }

