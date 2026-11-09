namespace Tmobis.Medium.WebApi
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Универсальная структура пагинированного результата списка сущностей.
    /// </summary>
    /// <typeparam name="T">Тип сущности.</typeparam>
    public sealed record PagedResult<T>(
        IReadOnlyList<T> Items,
        long? TotalCount);
}
