namespace CleanArchitecture.Blazor.Application.Features.TypeOfProfileMasterDatas.Specifications;
#nullable disable warnings
public class TypeOfProfileMasterDataByIdSpecification : Specification<TypeOfProfileMasterData>
{
    public TypeOfProfileMasterDataByIdSpecification(int id)
    {
       Query.Where(q => q.Id == id);
    }
}