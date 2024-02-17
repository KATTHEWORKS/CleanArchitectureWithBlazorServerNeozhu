using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Infrastructure.Extensions;
public class Repository<T> where T : class
{
    private readonly ApplicationDbContext _dbContext;
    // private readonly IServiceProvider _services;
    public Repository(IServiceProvider services)
    {
        //   _services = services;
        _dbContext = services.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    public Repository(ApplicationDbContext context)
    {
        _dbContext = context;
    }

    public async Task<int> UpdateColumnAsync<TProperty>(T entity, Expression<Func<T, TProperty>> propertyExpression, TProperty value)
    {
        var propertyName = ((MemberExpression)propertyExpression.Body).Member.Name;
        var entry = _dbContext.Entry(entity);
        entry.Property(propertyName).CurrentValue = value;
        entry.Property(propertyName).IsModified = true;
        entry.CurrentValues.SetValues(entity);
        return await _dbContext.SaveChangesAsync();
    }
}
