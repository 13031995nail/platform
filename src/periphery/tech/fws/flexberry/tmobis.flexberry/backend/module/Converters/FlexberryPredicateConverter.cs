namespace Tmobis.Frameworks.Flexberry.Converters;
using System;
using System.Collections.Generic;
using ICSSoft.STORMNET;
using ICSSoft.STORMNET.FunctionalLanguage;
using ICSSoft.STORMNET.Windows.Forms;
using Tmobis.Core.Converters;

public class FlexberryPredicateConverter(
    IPropertyNameConverter propertyNameConverter)
: IFlexberryPredicateConverter
{
    private readonly IPropertyNameConverter propertyNameConverter
    = propertyNameConverter;

    public virtual Function ConvertPredicate(
          Type type, string simplePredicateStr, List<string> propsForView)
    {
        if (string.IsNullOrWhiteSpace(simplePredicateStr))
        {
            return null;
        }

        var lD = ExternalLangDef.LanguageDef;
        Function result = null;
        var slittedVal = new List<string>();
        var startSymbol = 0;
        var endSymbol = 0;
        var part = string.Empty;
        for (var index = 0; index < simplePredicateStr.Length; index++)
        {
            var symbol = simplePredicateStr[index];
            if (symbol == '{')
            {
                startSymbol++;
                if (startSymbol - endSymbol == 1)
                {
                    continue;
                }
            }
            else if (symbol == '}')
            {
                endSymbol++;
                if (startSymbol - endSymbol == 0)
                {
                    continue;
                }
            }
            else if (symbol == '|' && startSymbol - endSymbol == 1)
            {
                slittedVal.Add(part);
                part = string.Empty;
                continue;
            }

            part += symbol;
        }

        slittedVal.Add(part);
        var attrCount = slittedVal.Count;
        if (attrCount < 3)
        {
            return result;
        }

        var predicateType = slittedVal[0];

        switch (predicateType)
        {
            case "SimpleP":
            case "DateP":
                if (attrCount == 5)
                {
                    object value = slittedVal[4];
                    switch (slittedVal[3])
                    {
                        case "number":
                            if (value == "NaN")
                            {
                                value = null;
                            }
                            else
                            {
                                value = Convert.ToDecimal(value);
                            }

                            break;
                        case "object":
                            if (value == "null" || value == null)
                            {
                                value = null;
                            }
                            else
                            {
                                return result;
                            }

                            break;
                        case "undefined":
                        case "boolean":
                        case "string":
                            break;
                    }

                    return GetFunction(
                        type, slittedVal[1], value, slittedVal[2], propsForView);
                }
                else
                {
                    throw new Exception("Указаны неверные параметры предиката.");
                }

            case "ComplexP":
                if (attrCount >= 4)
                {
                    var condition = slittedVal[1];
                    var predicates = new List<Function>();
                    foreach (var attr in slittedVal)
                    {
                        if (attr != predicateType && attr != condition)
                        {
                            predicates.Add(
                                ConvertPredicate(type, attr, propsForView));
                        }
                    }

                    return lD.GetFunction(
                        GetCondition(condition), [.. predicates]);
                }
                else
                {
                    throw new Exception("Указаны неверные параметры предиката.");
                }

            case "StringP":
                if (attrCount == 3)
                {
                    return lD.GetFunction(
                        lD.funcLike,
                        new VariableDef(lD.StringType, slittedVal[1]),
                        slittedVal[2]);
                }
                else
                {
                    throw new Exception("Указаны неверные параметры предиката.");
                }

            default:
                throw new Exception("Указаны неверные параметры предиката.");
        }
    }

    protected virtual string GetOperation(string strOperation)
    {
        if (string.IsNullOrWhiteSpace(strOperation))
        {
            return null;
        }

        var strOp = strOperation.Trim().ToUpper();
        if (strOp == "EQ")
        {
            return ExternalLangDef.LanguageDef.funcEQ;
        }
        else if (strOp == "NEQ")
        {
            return ExternalLangDef.LanguageDef.funcNEQ;
        }
        else if (strOp == "GE")
        {
            return ExternalLangDef.LanguageDef.funcG;
        }
        else if (strOp == "GEQ")
        {
            return ExternalLangDef.LanguageDef.funcGEQ;
        }
        else if (strOp == "LE")
        {
            return ExternalLangDef.LanguageDef.funcL;
        }
        else if (strOp == "LEQ")
        {
            return ExternalLangDef.LanguageDef.funcLEQ;
        }

        return null;
    }

    protected virtual string GetCondition(string strCondition)
    {
        if (string.IsNullOrWhiteSpace(strCondition))
        {
            return null;
        }

        var strCond = strCondition.Trim().ToUpper();
        if (strCond == ExternalLangDef.LanguageDef.funcAND)
        {
            return ExternalLangDef.LanguageDef.funcAND;
        }
        else if (strCond == ExternalLangDef.LanguageDef.funcOR)
        {
            return ExternalLangDef.LanguageDef.funcOR;
        }

        return null;
    }

    protected virtual Function GetFunction(
        Type type,
        string propName,
        object value,
        string operation,
        List<string> propsForView)
    {
        var propNameS = propertyNameConverter.GetBackendPropertyName(propName);
        var op = GetOperation(operation);
        var t = GetObjectType(type, propNameS);
        if (!propsForView.Contains(propNameS))
        {
            propsForView.Add(propNameS);
        }

        if (t == ExternalLangDef.LanguageDef.DataObjectType)
        {
            t = ExternalLangDef.LanguageDef.GuidType;
            //propNameS += ".__PrimaryKey";
        }

        return ExternalLangDef.LanguageDef
            .GetFunction(op, new VariableDef(t, propNameS), value);
    }

    protected virtual ObjectType GetObjectType(Type type, string propPath)
    {
        var curType = type;
        var curPropName = propPath;
        if (propPath.Contains("."))
        {
            var paths = propPath.Split('.');
            for (var i = 0; i < paths.Length - 1; i++)
            {
                curPropName = paths[i];
                var prop = curType.GetProperty(curPropName);
                if (prop == null)
                {
                    break;
                }

                curType = prop.PropertyType;
                curPropName = paths[i + 1];
            }
        }

        var propInfo = curType.GetProperty(curPropName);
        if (propInfo == null)
        {
            throw new Exception("Неверный путь при конвертировании предиката.");
        }

        if (IsDataObjectType(propInfo.PropertyType))
        {
            return ExternalLangDef.LanguageDef.DataObjectType;
        }
        else if (propInfo.PropertyType == typeof(string))
        {
            return ExternalLangDef.LanguageDef.StringType;
        }
        else if (propInfo.PropertyType == typeof(bool)
            || propInfo.PropertyType == typeof(bool?))
        {
            return ExternalLangDef.LanguageDef.BoolType;
        }
        else if (propInfo.PropertyType == typeof(decimal)
            || propInfo.PropertyType == typeof(decimal?)
            || propInfo.PropertyType == typeof(int)
            || propInfo.PropertyType == typeof(int?))
        {
            return ExternalLangDef.LanguageDef.NumericType;
        }

        throw new Exception("Неверный путь и тип при конвертировании предиката.");
    }

    protected virtual bool IsDataObjectType(Type type)
    => type != null
            && (type.BaseType == typeof(DataObject)
                || IsDataObjectType(type.BaseType));
}