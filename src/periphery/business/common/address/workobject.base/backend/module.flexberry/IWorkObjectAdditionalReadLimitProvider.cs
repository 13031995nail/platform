namespace Tmobis.Business.Common.Address;

using ICSSoft.STORMNET.FunctionalLanguage;

/// <summary>
/// Получение дополнительных полномочий чтения без изменения основных ролевых границ.
/// </summary>
public interface IWorkObjectAdditionalReadLimitProvider
{
    /// <summary>
    /// Построение ограничения; null означает отсутствие дополнительных полномочий.
    /// </summary>
    Task<Function?> GetReadLimit(string workObjectPropertyPath, string? awpPropertyPath = null);
}
