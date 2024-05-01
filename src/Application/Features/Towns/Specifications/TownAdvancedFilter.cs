namespace CleanArchitecture.Blazor.Application.Features.Towns.Specifications;
#nullable disable warnings
public enum TownListView
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

public class TownAdvancedFilter: PaginationFilter
{
    public TownListView ListView { get; set; } = TownListView.All;
    public UserProfile? CurrentUser { get; set; }
}