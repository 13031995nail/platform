using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tmobis.Objects.EF.Migrations
{
    /// <inheritdoc />
    public partial class Audit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "auditevent",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    eventtype = table.Column<int>(type: "integer", nullable: false),
                    entityid = table.Column<Guid>(type: "uuid", nullable: false),
                    entitytype = table.Column<string>(type: "text", nullable: false),
                    entitysnapshot = table.Column<string>(type: "jsonb", nullable: false),
                    userid = table.Column<Guid>(type: "uuid", nullable: false),
                    source = table.Column<string>(type: "text", nullable: true),
                    createtime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    creator = table.Column<string>(type: "text", nullable: true),
                    edittime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    editor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_auditevent", x => x.id);
                    table.ForeignKey(
                        name: "fk_auditevent_user_userid",
                        column: x => x.userid,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_auditevent_userid",
                table: "auditevent",
                column: "userid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "auditevent");
        }
    }
}
