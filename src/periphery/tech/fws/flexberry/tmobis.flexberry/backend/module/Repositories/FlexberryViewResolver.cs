namespace Tmobis.Frameworks.Flexberry.Repositories;

using ICSSoft.STORMNET;
using System;
using Tmobis.Core.Entities;

/// <summary>
/// Содержит общую логику выбора Flexberry View для репозиториев.
/// </summary>
internal static class FlexberryViewResolver
{
    /// <summary>
    /// Возвращает View по имени из IEntityView или edit-view по умолчанию, если view не передан или имя пустое.
    /// </summary>
    public static View Resolve(Type storedType, IEntityView? view)
    {
        if (view == null || string.IsNullOrWhiteSpace(view.ViewName))
        {
            return GetEditView(storedType);
        }

        return Information.GetView(view.ViewName, storedType);
    }

    /// <summary>
    /// Ищет edit-view по соглашению "{ИмяТипа}E" или создаёт View со всеми полями, если он не найден.
    /// </summary>
    public static View GetEditView(Type storedType)
    {
        var editView = Information.GetView($"{storedType.Name}E", storedType);
        if (editView != null)
        {
            return editView;
        }

        var view = new View { DefineClassType = storedType };
        view.AddProperty("*");
        return view;
    }
}
