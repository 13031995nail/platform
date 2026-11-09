using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tmobis.Integrations.Email.Service.Migrations
{
    /// <inheritdoc />
    public partial class AddSendEmailLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "changelog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    added = table.Column<string>(type: "text", nullable: false),
                    changes = table.Column<string>(type: "text", nullable: false),
                    fixes = table.Column<string>(type: "text", nullable: false),
                    hotfix = table.Column<string>(type: "text", nullable: false),
                    updatedate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    versiondate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    versionname = table.Column<string>(type: "text", nullable: false),
                    versionnumber = table.Column<string>(type: "text", nullable: false),
                    createtime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    creator = table.Column<string>(type: "text", nullable: true),
                    edittime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    editor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_changelog", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "file",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    size = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    isdeleteprotected = table.Column<bool>(type: "boolean", nullable: false),
                    createtime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    creator = table.Column<string>(type: "text", nullable: true),
                    edittime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    editor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_file", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "role",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    createtime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    creator = table.Column<string>(type: "text", nullable: true),
                    edittime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    editor = table.Column<string>(type: "text", nullable: true),
                    actuality = table.Column<bool>(type: "boolean", nullable: false),
                    code = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sendemaillog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    addresses = table.Column<List<string>>(type: "text[]", nullable: false),
                    success = table.Column<bool>(type: "boolean", nullable: false),
                    createtime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    creator = table.Column<string>(type: "text", nullable: true),
                    edittime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    editor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sendemaillog", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "town",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    houroffset = table.Column<int>(type: "integer", nullable: true),
                    cityname = table.Column<string>(type: "text", nullable: false),
                    cityacronym = table.Column<string>(type: "text", nullable: false),
                    comment = table.Column<string>(type: "text", nullable: true),
                    createtime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    creator = table.Column<string>(type: "text", nullable: true),
                    edittime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    editor = table.Column<string>(type: "text", nullable: true),
                    actuality = table.Column<bool>(type: "boolean", nullable: false),
                    code = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_town", x => x.id);
                });

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
                name: "user",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    login = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: true),
                    actuality = table.Column<bool>(type: "boolean", nullable: false),
                    userid = table.Column<string>(type: "text", nullable: true),
                    townid = table.Column<Guid>(type: "uuid", nullable: false),
                    pwdedittime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    pwd1 = table.Column<string>(type: "text", nullable: true),
                    pwd2 = table.Column<string>(type: "text", nullable: true),
                    iscomplexpwd = table.Column<bool>(type: "boolean", nullable: true),
                    userpwd = table.Column<string>(type: "text", nullable: false),
                    createtime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    creator = table.Column<string>(type: "text", nullable: true),
                    edittime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    editor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_town_townid",
                        column: x => x.townid,
                        principalTable: "town",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "employee",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "text", nullable: true),
                    firstname = table.Column<string>(type: "text", nullable: true),
                    lastname = table.Column<string>(type: "text", nullable: true),
                    patronymic = table.Column<string>(type: "text", nullable: true),
                    phone = table.Column<string>(type: "text", nullable: true),
                    photos = table.Column<string>(type: "text", nullable: true),
                    userid = table.Column<Guid>(type: "uuid", nullable: true),
                    townid = table.Column<Guid>(type: "uuid", nullable: false),
                    createtime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    creator = table.Column<string>(type: "text", nullable: true),
                    edittime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    editor = table.Column<string>(type: "text", nullable: true),
                    actuality = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_employee", x => x.id);
                    table.ForeignKey(
                        name: "fk_employee_town_townid",
                        column: x => x.townid,
                        principalTable: "town",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_employee_user_userid",
                        column: x => x.userid,
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "likedfolder",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    userid = table.Column<Guid>(type: "uuid", nullable: false),
                    infavorites = table.Column<string>(type: "text", nullable: false),
                    createtime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    creator = table.Column<string>(type: "text", nullable: true),
                    edittime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    editor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_likedfolder", x => x.id);
                    table.ForeignKey(
                        name: "fk_likedfolder_user_userid",
                        column: x => x.userid,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_employee_townid",
                table: "employee",
                column: "townid");

            migrationBuilder.CreateIndex(
                name: "ix_employee_userid",
                table: "employee",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "ix_likedfolder_userid",
                table: "likedfolder",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "ix_user_townid",
                table: "user",
                column: "townid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "changelog");

            migrationBuilder.DropTable(
                name: "employee");

            migrationBuilder.DropTable(
                name: "file");

            migrationBuilder.DropTable(
                name: "likedfolder");

            migrationBuilder.DropTable(
                name: "role");

            migrationBuilder.DropTable(
                name: "sendemaillog");

            migrationBuilder.DropTable(
                name: "userpassword");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "town");
        }
    }
}
