namespace Tmobis.Data.EF;

using Microsoft.EntityFrameworkCore;

public interface IDbContextConfigurator
{
    public void Build(DbContextOptionsBuilder options);
    
    public void Model(ModelBuilder modelBuilder);
}