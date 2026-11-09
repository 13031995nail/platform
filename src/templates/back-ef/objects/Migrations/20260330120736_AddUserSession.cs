using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tmobis.Objects.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddUserSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "userpassword",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    pwdhash = table.Column<string>(type: "text", nullable: false),
                    userid = table.Column<Guid>(type: "uuid", nullable: false),
                    createtime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    creator = table.Column<string>(type: "text", nullable: true),
                    edittime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    editor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_userpassword", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "usersession",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    startedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    closedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    userid = table.Column<Guid>(type: "uuid", nullable: false),
                    deviceinfo = table.Column<string>(type: "text", nullable: true),
                    login = table.Column<string>(type: "text", nullable: false),
                    ip = table.Column<string>(type: "text", nullable: false),
                    createtime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    creator = table.Column<string>(type: "text", nullable: true),
                    edittime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    editor = table.Column<string>(type: "text", nullable: true),
                    actuality = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_usersession", x => x.id);
                    table.ForeignKey(
                        name: "fk_usersession_user_userid",
                        column: x => x.userid,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_usersession_userid",
                table: "usersession",
                column: "userid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "userpassword");

            migrationBuilder.DropTable(
                name: "usersession");
        }
    }
}
