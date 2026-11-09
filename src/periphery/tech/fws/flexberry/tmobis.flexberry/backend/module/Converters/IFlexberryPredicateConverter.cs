namespace Tmobis.Frameworks.Flexberry.Converters;
using System;
using System.Collections.Generic;
using ICSSoft.STORMNET.FunctionalLanguage;

public interface IFlexberryPredicateConverter
{
    Function ConvertPredicate(
        Type type, string simplePredicateStr, List<string> propsForView);
}