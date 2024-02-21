namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.Constituencies.Specifications;
#nullable disable warnings
public enum ConstituencyListView
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

public class ConstituencyAdvancedFilter: PaginationFilter
{
    public ConstituencyListView ListView { get; set; } = ConstituencyListView.All;
    public UserProfile? CurrentUser { get; set; }
}