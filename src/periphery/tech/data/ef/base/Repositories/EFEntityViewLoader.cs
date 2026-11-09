namespace Tmobis.Data.EF.Repositories;

using Tmobis.Core.Entities;
using Tmobis.Core.Reflection;

/// <summary>
/// Догрузка навигаций в сущностях по IEntityView.
/// Использует метаданные EF для определения типа связи.
/// </summary>
public interface IEFEntityViewLoader
{
    Task LoadAsync(
        object entity,
        IEntityView view,
        CancellationToken cancellationToken = default);
}

public sealed class EFEntityViewLoader(IDbContext dbContext)
    : IEFEntityViewLoader
{
    public async Task LoadAsync(
        object entity,
        IEntityView view,
        CancellationToken cancellationToken = default)
    {
        foreach (var propertyPath in view.References)
        {
            await PropertyExtractor.ForEachPropertyInPath(
                entity,
                propertyPath,
                async (pathEntity, _, pathPropertyName) =>
                {
                    if (pathEntity == null)
                        return;

                    var entry = dbContext.Entry(pathEntity);

                    var navigation = entry.Metadata.FindNavigation(pathPropertyName);
                    if (navigation != null)
                    {
                        if (navigation.IsCollection)
                        {
                            var collection = entry.Collection(pathPropertyName);
                            if (!collection.IsLoaded)
                                await collection.LoadAsync(cancellationToken);
                        }
                        else
                        {
                            var reference = entry.Reference(pathPropertyName);
                            if (!reference.IsLoaded)
                                await reference.LoadAsync(cancellationToken);
                        }

                        return;
                    }

                    var skipNavigation = entry.Metadata.FindSkipNavigation(pathPropertyName);
                    if (skipNavigation != null)
                    {
                        var collection = entry.Collection(pathPropertyName);
                        if (!collection.IsLoaded)
                            await collection.LoadAsync(cancellationToken);
                    }
                });
        }
    }
}
