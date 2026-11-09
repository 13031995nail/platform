namespace Tmobis.Frameworks.Web.Backend;

public interface IHttpContextIpProvider
{
    /// <summary>
    /// Получить адрес текущего клиента.
    /// </summary>
    /// <returns>Адрес текущего клиента.</returns>
    public string? GetCurrentConnectionAddress();
}
