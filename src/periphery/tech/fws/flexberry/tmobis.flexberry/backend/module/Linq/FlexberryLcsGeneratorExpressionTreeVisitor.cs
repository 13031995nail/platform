namespace Tmobis.Frameworks.Flexberry.Linq;

using System.Collections.Generic;
using System.Linq.Expressions;
using ICSSoft.STORMNET;
using ICSSoft.STORMNET.Business.LINQProvider;
using ICSSoft.STORMNET.FunctionalLanguage;
using ICSSoft.STORMNET.FunctionalLanguage.SQLWhere;
using Tmobis.Core.Models;

/// <inheritdoc/>
public class FlexberryLcsGeneratorExpressionTreeVisitor
: LcsGeneratorExpressionTreeVisitorCommon
{
    private readonly bool viewIsDynamic;
    private readonly View view;

    public FlexberryLcsGeneratorExpressionTreeVisitor(
        TreeVisitorStacksHolder stacksHolder)
      : base(stacksHolder)
    {
    }

    public FlexberryLcsGeneratorExpressionTreeVisitor(
      bool viewIsDynamic, View view, IEnumerable<View> resolvingViews)
    : base(viewIsDynamic, view, resolvingViews)
    {
        this.viewIsDynamic = viewIsDynamic;
        this.view = view;
    }

    public override IQueryModelVisitor GetQueryModelVisitor(
        bool viewIsDynamic, View view, IEnumerable<View> resolvingViews)
    => new FlexberryLcsGeneratorQueryModelVisitor(
        viewIsDynamic, view, resolvingViews);

    protected virtual bool DataObjectMember
    {
        get
        => (bool)typeof(LcsGeneratorExpressionTreeVisitorBase)
                .GetField("_dataobjectmember",
                    System.Reflection.BindingFlags.Instance
                    | System.Reflection.BindingFlags.NonPublic).GetValue(this);

        set
        => typeof(LcsGeneratorExpressionTreeVisitorBase)
                .GetField("_dataobjectmember",
                    System.Reflection.BindingFlags.Instance
                    | System.Reflection.BindingFlags.NonPublic)?
                .SetValue(this, value);
    }

    protected override Expression VisitMember(MemberExpression expression)
    {
        Visit(expression.Expression);
        return VisitMemberInternal(expression);
    }

    protected virtual Expression VisitMemberInternal(
        MemberExpression expression)
    {
        var declaringType = expression.Member.DeclaringType;
        var memberType = Information.GetPropertyType(
            declaringType, expression.Member.Name);
        
        if (expression.Member.Name == nameof(IEntity.Id))
        {
            var varname = PopMasterMemberName(expression, out var master);
            VisitIdProperty(expression.Member.Name, master, varname);
            return expression;
        }
        else if (memberType.IsAssignableTo(typeof(IEntity)))
        {
            var varname = PopMasterMemberName(expression, out var master);
            UtilsLcs.AddPropertyToView(view, varname, viewIsDynamic);
            DataObjectMember = true;
            _stacksHolder.PushParam(
                new VariableDef(_ldef.DataObjectType, varname));
            return expression;
        }
        else
        {
            return VisitMemberExpressionHelper(expression);
        }
    }

    protected virtual string PopMasterMemberName(
        MemberExpression expression, out object master)
    {
        string varName;
        master = null;
        if (!DataObjectMember)
        {
            varName = expression.Member.Name;
        }
        else
        {
            var par = (VariableDef)_stacksHolder.PopParam();
            master = par;
            varName = $"{par.StringedView}.{expression.Member.Name}";
            DataObjectMember = false;
        }

        return varName;
    }

    protected virtual void VisitIdProperty(
        string expressionMemberName, object master, string varname)
    {
        if (master is VariableDef def)
        {
            UtilsLcs.AddMasterToView(view, varname, viewIsDynamic);
            var varDefMaster = def;
            _stacksHolder.PushParam(
                new VariableDef(_ldef.GuidType, varDefMaster.StringedView));
        }
        else
        {
            _stacksHolder.PushParam(new VariableDef(
                _ldef.GuidType,
                SQLWhereLanguageDef.StormMainObjectKey));
        }
    }
}
