using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tmobis.Objects.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkShiftStatuses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                INSERT INTO workshiftstatus (id, actuality, code, name)
                VALUES
                    ('7f3e8a12-4b5c-4d6e-9f80-1a2b3c4d5e03', TRUE, 'inProgress', 'В работе'),
                    ('7f3e8a12-4b5c-4d6e-9f80-1a2b3c4d5e04', TRUE, 'completed', 'Выполнено'),
                    ('7f3e8a12-4b5c-4d6e-9f80-1a2b3c4d5e05', TRUE, 'postponed', 'Перенесено')
                ON CONFLICT (id) DO NOTHING;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE workshift
                SET workshiftstatusid = '7f3e8a12-4b5c-4d6e-9f80-1a2b3c4d5e02'
                WHERE workshiftstatusid IN (
                    '7f3e8a12-4b5c-4d6e-9f80-1a2b3c4d5e03',
                    '7f3e8a12-4b5c-4d6e-9f80-1a2b3c4d5e04',
                    '7f3e8a12-4b5c-4d6e-9f80-1a2b3c4d5e05'
                );

                UPDATE workshifttask
                SET workshiftstatusid = '7f3e8a12-4b5c-4d6e-9f80-1a2b3c4d5e02'
                WHERE workshiftstatusid IN (
                    '7f3e8a12-4b5c-4d6e-9f80-1a2b3c4d5e03',
                    '7f3e8a12-4b5c-4d6e-9f80-1a2b3c4d5e04',
                    '7f3e8a12-4b5c-4d6e-9f80-1a2b3c4d5e05'
                );

                DELETE FROM workshiftstatus
                WHERE id IN (
                    '7f3e8a12-4b5c-4d6e-9f80-1a2b3c4d5e03',
                    '7f3e8a12-4b5c-4d6e-9f80-1a2b3c4d5e04',
                    '7f3e8a12-4b5c-4d6e-9f80-1a2b3c4d5e05'
                );
                """);
        }
    }
}
