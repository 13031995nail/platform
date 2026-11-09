namespace Tmobis.PrimaryAdapters.BaseWebService.Interfaces;

using System.Runtime.Serialization;

/// <summary>
/// Ответ веб-сервиса.
/// </summary>
public interface IWebServiceResponse
{
  /// <summary>
  /// Статус ответа.
  /// </summary>
  [DataMember(Name = "requestGuid")]
  public string Status { get; set; }

  /// <summary>
  /// Код ошибки.
  /// </summary>
  [DataMember(Name = "requestGuid")]
  public string ErrorCode { get; set; }

  /// <summary>
  /// Сообщение об ошибке.
  /// </summary>
  [DataMember(Name = "requestGuid")]
  public string ErrorMessage { get; set; }
}
