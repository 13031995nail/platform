namespace Tmobis.Business.Process.Documents.Flexberry;

using ICSSoft.STORMNET;

/// <summary>
/// Ограничение разрешения отмены проверенным документом и его загруженными подписями.
/// </summary>
public sealed class ActSigningCancellationScope : IDisposable
{
    private static readonly AsyncLocal<ActSigningCancellationScope?> Current = new();
    private readonly ActSigningCancellationScope? previous;
    private readonly DataObject document;
    private readonly HashSet<ActSign> signs;

    private ActSigningCancellationScope(DataObject document, IEnumerable<ActSign> signs)
    {
        this.document = document;
        this.signs = signs.ToHashSet();
        previous = Current.Value;
        Current.Value = this;
    }

    public static IDisposable Enter(DataObject document, IEnumerable<ActSign> signs)
        => new ActSigningCancellationScope(document, signs);

    public static bool AllowsDocument(DataObject document)
        => ReferenceEquals(Current.Value?.document, document)
            && document.GetStatus() == ObjectStatus.Altered
            && document.GetAlteredPropertyNames().All(property =>
                property is "ActStatus" or "ChangeStatusDate");

    public static bool AllowsSign(ActSign sign)
        => Current.Value?.signs.Any(item => ReferenceEquals(item, sign)) == true
            && sign.GetStatus() == ObjectStatus.Altered
            && sign.SignDate == null && string.IsNullOrEmpty(sign.Signature)
            && sign.GetAlteredPropertyNames().All(property =>
                property is nameof(ActSign.SignDate) or nameof(ActSign.Signature));

    public void Dispose() => Current.Value = previous;
}
