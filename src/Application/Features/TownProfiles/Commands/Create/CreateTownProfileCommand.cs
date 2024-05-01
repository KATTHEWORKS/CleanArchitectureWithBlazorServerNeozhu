// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using System.ComponentModel;
using CleanArchitecture.Blazor.Application.Features.TownProfiles.DTOs;
using CleanArchitecture.Blazor.Application.Features.TownProfiles.Caching;

namespace CleanArchitecture.Blazor.Application.Features.TownProfiles.Commands.Create;

public class CreateTownProfileCommand: ICacheInvalidatorRequest<Result<int>>
{
      [Description("Id")]
      public int Id { get; set; }
          [Description("Type Id")]
    public int TypeId {get;set;} 
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
    [Description("Town Id")]
    public int TownId {get;set;} 
    [Description("Approved Count")]
    public int ApprovedCount {get;set;} 
    [Description("Rejected Count")]
    public int RejectedCount {get;set;} 
    [Description("Like Count")]
    public int LikeCount {get;set;} 
    [Description("Dis Like Count")]
    public int DisLikeCount {get;set;} 

      public string CacheKey => TownProfileCacheKey.GetAllCacheKey;
      public CancellationTokenSource? SharedExpiryTokenSource => TownProfileCacheKey.SharedExpiryTokenSource();
    private class Mapping : Profile
    {
        public Mapping()
        {
             CreateMap<TownProfileDto,CreateTownProfileCommand>(MemberList.None);
             CreateMap<CreateTownProfileCommand,TownProfile>(MemberList.None);
        }
    }
}
    
    public class CreateTownProfileCommandHandler : IRequestHandler<CreateTownProfileCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<CreateTownProfileCommand> _localizer;
        public CreateTownProfileCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<CreateTownProfileCommand> localizer,
            IMapper mapper
            )
        {
            _context = context;
            _localizer = localizer;
            _mapper = mapper;
        }
        public async Task<Result<int>> Handle(CreateTownProfileCommand request, CancellationToken cancellationToken)
        {
           var item = _mapper.Map<TownProfile>(request);
           // raise a create domain event
	       item.AddDomainEvent(new TownProfileCreatedEvent(item));
           _context.TownProfiles.Add(item);
           await _context.SaveChangesAsync(cancellationToken);
           return  await Result<int>.SuccessAsync(item.Id);
        }
    }

