using AutoMapper;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;
using CleanArchitecture.Blazor.Domain.Identity;
using CleanArchitecture.Blazor.Infrastructure.Extensions;
using LazyCache;
using ZiggyCreatures.Caching.Fusion;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Identity;

public class UserService : IUserService
{
    //Its bad descision to store all users at one place,so all referred place taking out
    private const string CACHEKEY = "ALL-ApplicationUserDto";
    private readonly IFusionCache _fusionCache;
    private readonly IMapper _mapper;
    private readonly CustomUserManager _userManager;
    public List<ApplicationUserDto> DataSource { get; private set; }
    //This loads all users at a time,its wrong design so disabling all
    //instead it can be made like fetch particular user and keep in cache in stack & upto some 100 like if 101 comes clear first earlier kind of
    //till then commenting the code usage
    public UserService(
        IFusionCache fusionCache,
        IMapper mapper,
        IServiceScopeFactory scopeFactory)
    {
        _fusionCache = fusionCache;
        _mapper = mapper;
        var scope = scopeFactory.CreateScope();
        _userManager = scope.ServiceProvider.GetRequiredService<CustomUserManager>();
        DataSource = new List<ApplicationUserDto>();
    }

    public event Action? OnChange;

    public void Initialize()
    {

        DataSource = _fusionCache.GetOrSet(CACHEKEY,
            _ => _userManager.Users.Include(x => x.UserRoleTenants).ThenInclude(x => x.Role)
                .ProjectTo<ApplicationUserDto>(_mapper.ConfigurationProvider).OrderBy(x => x.UserName).ToList())
            ?? new List<ApplicationUserDto>();
        OnChange?.Invoke();
    }

    

    public void Refresh()
    {
        _fusionCache.Remove(CACHEKEY);
        DataSource = _fusionCache.GetOrSet(CACHEKEY,
             _ => _userManager.Users.Include(x => x.UserRoleTenants).ThenInclude(x => x.Role)
                 .ProjectTo<ApplicationUserDto>(_mapper.ConfigurationProvider).OrderBy(x => x.UserName).ToList())
             ?? new List<ApplicationUserDto>();
        OnChange?.Invoke();
    }
}