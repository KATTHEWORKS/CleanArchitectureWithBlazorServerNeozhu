namespace CleanArchitecture.Blazor.Application.Features.CardTypes.Specifications;

#nullable disable warnings
/// <summary>
/// Specifies the different views available for the CardType list.
/// </summary>
public enum CardTypeListView
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
/// <summary>
/// A class for applying advanced filtering options to CardType lists.
/// </summary>
public class CardTypeAdvancedFilter: PaginationFilter
{
    public CardTypeListView ListView { get; set; } = CardTypeListView.All;
    public UserProfile? CurrentUser { get; set; }
}