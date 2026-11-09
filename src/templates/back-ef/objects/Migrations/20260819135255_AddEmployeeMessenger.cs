using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tmobis.Objects.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeMessenger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "preferredmessenger",
                table: "user",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "employeemessenger",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    employeeid = table.Column<Guid>(type: "uuid", nullable: false),
                    messenger = table.Column<int>(type: "integer", nullable: false),
                    messengerid = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    createtime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    creator = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    edittime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    editor = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_employeemessenger", x => x.id);
                    table.ForeignKey(
                        name: "fk_employeemessenger_employee_employeeid",
                        column: x => x.employeeid,
                        principalTable: "employee",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notificationprovidertype",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    createtime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    creator = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    edittime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    editor = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    actuality = table.Column<bool>(type: "boolean", nullable: false),
                    code = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notificationprovidertype", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "notificationtype",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    createtime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    creator = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    edittime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    editor = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    actuality = table.Column<bool>(type: "boolean", nullable: false),
                    code = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notificationtype", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "notification",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    userid = table.Column<Guid>(type: "uuid", nullable: false),
                    readingtime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    typeid = table.Column<Guid>(type: "uuid", nullable: false),
                    createtime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    creator = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    edittime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    editor = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notification", x => x.id);
                    table.ForeignKey(
                        name: "fk_notification_notificationtype_typeid",
                        column: x => x.typeid,
                        principalTable: "notificationtype",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_notification_user_userid",
                        column: x => x.userid,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notificationcontent",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    providertypeid = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    notificationid = table.Column<Guid>(type: "uuid", nullable: false),
                    createtime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    creator = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    edittime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    editor = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notificationcontent", x => x.id);
                    table.ForeignKey(
                        name: "fk_notificationcontent_notification_notificationid",
                        column: x => x.notificationid,
                        principalTable: "notification",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_notificationcontent_notificationprovidertype_providertypeid",
                        column: x => x.providertypeid,
                        principalTable: "notificationprovidertype",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_employeemessenger_employeeid",
                table: "employeemessenger",
                column: "employeeid");

            migrationBuilder.CreateIndex(
                name: "ix_employeemessenger_employeeid_messenger",
                table: "employeemessenger",
                columns: new[] { "employeeid", "messenger" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_employeemessenger_messenger_messengerid",
                table: "employeemessenger",
                columns: new[] { "messenger", "messengerid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_notification_typeid",
                table: "notification",
                column: "typeid");

            migrationBuilder.CreateIndex(
                name: "ix_notification_userid",
                table: "notification",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "ix_notificationcontent_notificationid",
                table: "notificationcontent",
                column: "notificationid");

            migrationBuilder.CreateIndex(
                name: "ix_notificationcontent_providertypeid",
                table: "notificationcontent",
                column: "providertypeid");

            migrationBuilder.InsertData(
                table: "notificationprovidertype",
                columns: new[] { "id", "actuality", "code", "name" },
                values: new object[]
                {
                    new Guid("f5cf53f4-c95d-4d4c-a74f-46ca25db7300"),
                    true,
                    "telegram",
                    "Telegram"
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "employeemessenger");

            migrationBuilder.DropTable(
                name: "notificationcontent");

            migrationBuilder.DropTable(
                name: "notification");

            migrationBuilder.DropTable(
                name: "notificationprovidertype");

            migrationBuilder.DropTable(
                name: "notificationtype");

            migrationBuilder.DropColumn(
                name: "preferredmessenger",
                table: "user");
        }
    }
}
