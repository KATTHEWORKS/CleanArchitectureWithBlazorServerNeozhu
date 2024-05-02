﻿namespace CleanArchitecture.Blazor.Application.Features.TypeOfProfileMasterDatas.Specifications;
#nullable disable warnings
public class TypeOfProfileMasterDataAdvancedSpecification : Specification<TypeOfProfileMasterData>
{
    public TypeOfProfileMasterDataAdvancedSpecification(TypeOfProfileMasterDataAdvancedFilter filter)
    {
        var today = DateTime.Now.ToUniversalTime().Date;
        var start = Convert.ToDateTime(today.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 00:00:00",
            CultureInfo.CurrentCulture);
        var end = Convert.ToDateTime(today.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 23:59:59",
            CultureInfo.CurrentCulture);
        var last30day = Convert.ToDateTime(
            today.AddDays(-30).ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 00:00:00",
            CultureInfo.CurrentCulture);

       Query.Where(q => q.Name != null)
             .Where(q => q.Name!.Contains(filter.Keyword) || q.Description!.Contains(filter.Keyword), !string.IsNullOrEmpty(filter.Keyword))
             .Where(q => q.CreatedBy == filter.CurrentUser.UserId, filter.ListView == TypeOfProfileMasterDataListView.My && filter.CurrentUser is not null)
             .Where(q => q.Created >= start && q.Created <= end, filter.ListView == TypeOfProfileMasterDataListView.CreatedToday)
             .Where(q => q.Created >= last30day, filter.ListView == TypeOfProfileMasterDataListView.Created30Days);
       
    }
}