namespace Tmobis.Core.Mappers;

public interface IConcreteMapper<in TFrom, out TTo> : IMapperBase
{
    TTo Map(TFrom source);
}
