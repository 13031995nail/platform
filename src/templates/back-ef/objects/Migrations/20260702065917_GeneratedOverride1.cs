using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tmobis.Objects.EF.Migrations
{
    /// <inheritdoc />
    public partial class GeneratedOverride1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "fio",
                table: "employee");

            migrationBuilder.Sql(
                """
                ALTER TABLE auditevent
                ALTER COLUMN entitysnapshot TYPE jsonb
                USING entitysnapshot::jsonb;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "fio",
                table: "employee",
                type: "text",
                nullable: true);

            migrationBuilder.Sql(
                """
                ALTER TABLE auditevent
                ALTER COLUMN entitysnapshot TYPE text
                USING entitysnapshot::text;
                """);
        }
    }
}
