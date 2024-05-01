namespace CleanArchitecture.Blazor.Application.Features.TownProfiles.Specifications;
#nullable disable warnings
public class TownProfileByIdSpecification : Specification<TownProfile>
{
    public TownProfileByIdSpecification(int id)
    {
       Query.Where(q => q.Id == id);
    }
}