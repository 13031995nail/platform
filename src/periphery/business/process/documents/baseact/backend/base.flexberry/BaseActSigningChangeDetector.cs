namespace Tmobis.Business.Process.Documents.Base.Flexberry;

using ICSSoft.STORMNET;
using Tmobis.Business.Process.Documents.Base;

/// <summary>
/// Определяет изменения статуса и состава полей, относящиеся к подписанию.
/// </summary>
public static class BaseActSigningChangeDetector
{
    public static bool IsSigningStatusChange(
        BaseActBase document)
        => document.GetStatus() == ObjectStatus.Altered
            && document.IsAlteredProperty(nameof(document.ActStatus))
            && document.ActStatus != null
            && (document.ActStatus.Id == IActStatus.Keys.Signing
                || document.ActStatus.Id == IActStatus.Keys.Signed
                || document.ActStatus.Id == IActStatus.Keys.Approved);

    public static bool IsIsolatedSigningStatusChange(
        BaseActBase document,
        params string[] additionalAllowedProperties)
    {
        var allowedProperties = new HashSet<string>(additionalAllowedProperties)
        {
            nameof(document.ActStatus),
            nameof(document.ChangeStatusDate),
        };
        return document.GetAlteredPropertyNames().All(allowedProperties.Contains);
    }
}
