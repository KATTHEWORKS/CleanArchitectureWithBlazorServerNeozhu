namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.Specifications;
#nullable disable warnings
public enum VoteListView
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

public class VoteAdvancedFilter: PaginationFilter
{
    public VoteListView ListView { get; set; } = VoteListView.All;
    public UserProfile? CurrentUser { get; set; }
}