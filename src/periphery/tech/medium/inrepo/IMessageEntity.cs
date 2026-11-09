  namespace Tmobis.Medium.InRepo;
  using Tmobis.Core.Models;
  public interface IMessageEntity : IEntity
  {
    public Guid Id { get; set; }

    string RawMessage { get; set; }

  }

