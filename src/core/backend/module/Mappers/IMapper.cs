namespace Tmobis.Core.Mappers;

public interface IMapper : IMapperBase
{
    TTo Map<TFrom, TTo>(TFrom source);
}
