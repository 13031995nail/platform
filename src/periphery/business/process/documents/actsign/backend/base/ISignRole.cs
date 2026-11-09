namespace Tmobis.Business.Process.Documents;

using Tmobis.Core.Models;

public interface ISignRole : IDictionary
{
    /// <summary>
    /// Статические ключи сущностей
    /// </summary>
    public static class Keys
    {
        /// <summary>
        /// Представитель проверяемой организации
        /// </summary>
        public static Guid OrganizationRepresentative { get; } = new("fc83a517-d109-4c75-9ee3-a5fa52df0e94");

        /// <summary>
        /// Председатель комиссии
        /// </summary>
        public static Guid CommissionChair { get; } = new("fc83a517-d109-4c75-9ee3-a5fa52df0e93");

        /// <summary>
        /// Заместитель председателя комиссии
        /// </summary>
        public static Guid CommissionDeputyChair { get; } = new("fc83a517-d109-4c75-9ee3-a5fa52df0e96");

        /// <summary>
        /// Сотрудник ТСО
        /// </summary>
        public static Guid TSOemployee { get; } = new("fc83a517-d109-4c75-9ee3-a5fa52df0e95");

        /// <summary>
        /// Член комиссии
        /// </summary>
        public static Guid CommissionMember { get; } = new("fc83a517-d109-4c75-9ee3-a5fa52df0e92");

        /// <summary>
        /// Орган, образовавший комиссию
        /// </summary>
        public static Guid CommissionCreator { get; } = new("fc83a517-d109-4c75-9ee3-a5fa52df0e97");

    }
}
