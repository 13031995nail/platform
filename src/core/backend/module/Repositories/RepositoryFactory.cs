namespace Tmobis.Core.Repositories;

using Microsoft.Extensions.DependencyInjection;

public class RepositoryFactory(IServiceScopeFactory factory)
: IRepositoryFactory
{
    public RepositoryScope MakeRepository()
    {
        var scope = factory.CreateScope();
        var provider = scope.ServiceProvider;
        return new RepositoryScope(
            provider.GetRequiredService<IRepository>(),
            provider.GetRequiredService<IUnitOfWork>(),
            scope);
    }
}
