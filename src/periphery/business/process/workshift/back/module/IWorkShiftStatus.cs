namespace Tmobis.Business.Process.WorkShift;

using Tmobis.Core.Entities;
using Tmobis.Core.Models;

public interface IWorkShiftStatus : IDictionary, IRegisteredEntity
{
    /// <summary>
    /// Статические ключи сущностей.
    /// </summary>
    public static class Keys
    {
        /// <summary>
        /// Не утвержден.
        /// </summary>
        public static Guid NotApproved { get; } = new("7f3e8a12-4b5c-4d6e-9f80-1a2b3c4d5e01");

        /// <summary>
        /// Утвержден.
        /// </summary>
        public static Guid Approved { get; } = new("7f3e8a12-4b5c-4d6e-9f80-1a2b3c4d5e02");

        /// <summary>
        /// В работе.
        /// </summary>
        public static Guid InProgress { get; } = new("7f3e8a12-4b5c-4d6e-9f80-1a2b3c4d5e03");

        /// <summary>
        /// Выполнено.
        /// </summary>
        public static Guid Completed { get; } = new("7f3e8a12-4b5c-4d6e-9f80-1a2b3c4d5e04");

        /// <summary>
        /// Перенесено.
        /// </summary>
        public static Guid Postponed { get; } = new("7f3e8a12-4b5c-4d6e-9f80-1a2b3c4d5e05");
    }

    public class Views
    {
        public static readonly EntityView WorkShiftStatusL
            = new(
                nameof(WorkShiftStatusL),
                searchProperties: [
                    nameof(Name),
                ]);
    }
}
