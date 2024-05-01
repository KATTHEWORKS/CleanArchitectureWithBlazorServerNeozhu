namespace CleanArchitecture.Blazor.Application.Features.Towns.Specifications;
#nullable disable warnings
public class TownByIdSpecification : Specification<Town>
{
    public TownByIdSpecification(int id)
    {
       Query.Where(q => q.Id == id);
    }
}