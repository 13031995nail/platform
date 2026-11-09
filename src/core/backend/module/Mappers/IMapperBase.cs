namespace Tmobis.Core.Mappers;

using System;

public interface IMapperBase
{
    object Map(object source, Type targetType);
}
