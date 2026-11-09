namespace Tmobis.Business.Common.Sessions.Flexberry.Models;
using Tmobis.Business.Common.Sessions.Models;
using Tmobis.Frameworks.Flexberry.Entities;
using Tmobis.Business.Core.Models.Users;
using User = Core.Flexberry.Models.User;
using System.Runtime.Serialization;
using ICSSoft.STORMNET;

[AutoAltered]
[AccessType(AccessType.@this)]
[View("SessionD", ["*"])]
[TypeStorage("Session")]
[ClassStorage("Session")]
public class UserSession
    : FlexberryActualEntity, IUserSession
{
    public virtual DateTime StartedAt { get; set; }

    public virtual DateTime? ClosedAt { get; set; }

    [NotStored]
    public Guid UserId
    {
        get => User.Id;
        set {}
    }

    [PropertyStorage(["UserKey"])]
    public virtual User User { get; set; }

    public virtual string? DeviceInfo { get; set; }

    public virtual string Login { get; set; } = string.Empty;

    public virtual string Ip { get; set; } = string.Empty;

    [NotStored]
    [IgnoreDataMember]
    IUser IUserSession.User
    {
        get => User;
        set
        {
            if (value is User user)
            {
                User = user;
            }
        }
    }

    /// <summary>
    /// Class views container.
    /// </summary>
    public static class Views
    {
        /// <summary>
        /// "SessionD" view.
        /// </summary>
        public static View SessionD { get; }
            = Information.GetView("SessionD", typeof(UserSession));
    }
}