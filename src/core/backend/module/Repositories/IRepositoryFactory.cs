namespace Tmobis.Core.Repositories;

public interface IRepositoryFactory
{
    RepositoryScope MakeRepository();
}
