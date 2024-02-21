namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.VoteSummaries.Specifications;
#nullable disable warnings
public enum VoteSummaryListView
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

public class VoteSummaryAdvancedFilter: PaginationFilter
{
    public VoteSummaryListView ListView { get; set; } = VoteSummaryListView.All;
    public UserProfile? CurrentUser { get; set; }
}