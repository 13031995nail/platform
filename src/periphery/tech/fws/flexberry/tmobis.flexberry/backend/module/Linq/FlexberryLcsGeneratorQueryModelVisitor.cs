namespace Tmobis.Frameworks.Flexberry.Linq;

using ICSSoft.STORMNET;
using ICSSoft.STORMNET.Business.LINQProvider;

/// <inheritdoc/>
public class FlexberryLcsGeneratorQueryModelVisitor(
    bool viewIsDynamic, View view, IEnumerable<View> resolvingViews)
: LcsGeneratorQueryModelVisitor(viewIsDynamic, view, resolvingViews)
{
    /// <inheritdoc/>
    public override IExpressionTreeVisitor GetExpressionTreeVisitor(
        bool viewIsDynamic, View view, IEnumerable<View> resolvingViews)
    {
        return new FlexberryLcsGeneratorExpressionTreeVisitor(
            viewIsDynamic, view, resolvingViews);
    }
}