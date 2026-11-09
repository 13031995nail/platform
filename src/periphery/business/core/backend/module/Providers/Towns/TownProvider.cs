namespace Tmobis.Business.Core.Providers.Towns;

using Tmobis.Business.Core.Models.Towns;
using Tmobis.Core.Repositories;
using System;

/// <inheritdoc/>
public class TownProvider(IReadRepository repository)
    : ITownProvider
{
    /// <inheritdoc/>
    public async ValueTask<ITown?> GetTownByCode(string code)
    => await repository.FirstOrDefault<ITown>(
        town => town.Code == code && town.Actuality);

    /// <inheritdoc/>
    public async ValueTask<ITown?> GetTownById(Guid id)
    => await repository.GetById<ITown>(id);
}
