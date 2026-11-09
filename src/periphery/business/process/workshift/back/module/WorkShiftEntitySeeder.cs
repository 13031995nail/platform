namespace Tmobis.Business.Process.WorkShift;

using Tmobis.Core.Entities;

public sealed class WorkShiftEntitySeeder : IEntitySeeder
{
    /// <inheritdoc/>
    public void Seed(IEntitySeedBuilder builder)
    {
        builder.Add(
            new WorkShiftStatus
            {
                Id = IWorkShiftStatus.Keys.NotApproved,
                Code = "notApproved",
                Name = "Не утвержден",
                Actuality = true,
            },
            new WorkShiftStatus
            {
                Id = IWorkShiftStatus.Keys.Approved,
                Code = "approved",
                Name = "Утвержден",
                Actuality = true,
            },
            new WorkShiftStatus
            {
                Id = IWorkShiftStatus.Keys.InProgress,
                Code = "inProgress",
                Name = "В работе",
                Actuality = true,
            },
            new WorkShiftStatus
            {
                Id = IWorkShiftStatus.Keys.Completed,
                Code = "completed",
                Name = "Выполнено",
                Actuality = true,
            },
            new WorkShiftStatus
            {
                Id = IWorkShiftStatus.Keys.Postponed,
                Code = "postponed",
                Name = "Перенесено",
                Actuality = true,
            });
    }
}
