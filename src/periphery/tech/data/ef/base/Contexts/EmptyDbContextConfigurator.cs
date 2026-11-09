namespace Tmobis.Data.EF;

using Microsoft.EntityFrameworkCore;

public class EmptyDbContextConfigurator()
: IDbContextConfigurator
{
    public virtual void Build(DbContextOptionsBuilder options)
    {
    }

    public void Model(ModelBuilder modelBuilder)
    {
    }
}