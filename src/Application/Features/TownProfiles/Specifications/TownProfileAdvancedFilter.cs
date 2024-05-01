

namespace CleanArchitecture.Blazor.Application.Features.TownProfiles.Specifications;
#nullable disable warnings
public enum TownProfileListView
{
    [Description("All")]
    All,
    [Description("My")]
    My,
    [Description("Created Toady")]
    CreatedToday,
    [Description("Created within the last 30 days")]
    Created30Days
}

public class TownProfileAdvancedFilter: PaginationFilter
{
    public TownProfileListView ListView { get; set; } = TownProfileListView.All;
    public UserProfile? CurrentUser { get; set; }
}