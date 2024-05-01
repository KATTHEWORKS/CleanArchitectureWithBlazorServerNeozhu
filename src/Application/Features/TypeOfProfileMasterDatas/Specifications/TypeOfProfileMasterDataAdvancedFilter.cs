

namespace CleanArchitecture.Blazor.Application.Features.TypeOfProfileMasterDatas.Specifications;
#nullable disable warnings
public enum TypeOfProfileMasterDataListView
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

public class TypeOfProfileMasterDataAdvancedFilter: PaginationFilter
{
    public TypeOfProfileMasterDataListView ListView { get; set; } = TypeOfProfileMasterDataListView.All;
    public UserProfile? CurrentUser { get; set; }
}