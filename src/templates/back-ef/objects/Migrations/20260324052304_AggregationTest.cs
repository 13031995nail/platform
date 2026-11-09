using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tmobis.Objects.EF.Migrations
{
    /// <inheritdoc />
    public partial class AggregationTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_user_townid",
                table: "user");

            migrationBuilder.DropIndex(
                name: "ix_likedfolder_userid",
                table: "likedfolder");

            migrationBuilder.DropIndex(
                name: "ix_employee_townid",
                table: "employee");

            migrationBuilder.DropIndex(
                name: "ix_employee_userid",
                table: "employee");

            migrationBuilder.CreateTable(
                name: "testentity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    test = table.Column<string>(type: "text", nullable: false),
                    createtime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    creator = table.Column<string>(type: "text", nullable: true),
                    edittime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    editor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_testentity", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "testentity2",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    test2 = table.Column<string>(type: "text", nullable: false),
                    entityrefid = table.Column<Guid>(type: "uuid", nullable: false),
                    createtime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    creator = table.Column<string>(type: "text", nullable: true),
                    edittime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    editor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_testentity2", x => x.id);
                    table.ForeignKey(
                        name: "fk_testentity2_testentity_entityrefid",
                        column: x => x.entityrefid,
                        principalTable: "testentity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_user_townid",
                table: "user",
                column: "townid");

            migrationBuilder.CreateIndex(
                name: "ix_likedfolder_userid",
                table: "likedfolder",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "ix_employee_townid",
                table: "employee",
                column: "townid");

            migrationBuilder.CreateIndex(
                name: "ix_employee_userid",
                table: "employee",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "ix_testentity2_entityrefid",
                table: "testentity2",
                column: "entityrefid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "testentity2");

            migrationBuilder.DropTable(
                name: "testentity");

            migrationBuilder.DropIndex(
                name: "ix_user_townid",
                table: "user");

            migrationBuilder.DropIndex(
                name: "ix_likedfolder_userid",
                table: "likedfolder");

            migrationBuilder.DropIndex(
                name: "ix_employee_townid",
                table: "employee");

            migrationBuilder.DropIndex(
                name: "ix_employee_userid",
                table: "employee");

            migrationBuilder.CreateIndex(
                name: "ix_user_townid",
                table: "user",
                column: "townid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_likedfolder_userid",
                table: "likedfolder",
                column: "userid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_employee_townid",
                table: "employee",
                column: "townid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_employee_userid",
                table: "employee",
                column: "userid",
                unique: true);
        }
    }
}
