using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tmobis.Objects.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkShift : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "workshiftstatus",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    actuality = table.Column<bool>(type: "boolean", nullable: false),
                    code = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: false),
                    createtime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    creator = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    edittime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    editor = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_workshiftstatus", x => x.id);
                });

            migrationBuilder.Sql("""
                INSERT INTO workshiftstatus (id, actuality, code, name)
                VALUES
                    ('7f3e8a12-4b5c-4d6e-9f80-1a2b3c4d5e01', TRUE, 'notApproved', 'Не утвержден'),
                    ('7f3e8a12-4b5c-4d6e-9f80-1a2b3c4d5e02', TRUE, 'approved', 'Утвержден')
                ON CONFLICT (id) DO NOTHING;
                """);

            migrationBuilder.CreateTable(
                name: "workshift",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    employeeid = table.Column<Guid>(type: "uuid", nullable: false),
                    workshiftstatusid = table.Column<Guid>(type: "uuid", nullable: false),
                    createtime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    creator = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    edittime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    editor = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_workshift", x => x.id);
                    table.ForeignKey(
                        name: "fk_workshift_employee_employeeid",
                        column: x => x.employeeid,
                        principalTable: "employee",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_workshift_workshiftstatus_workshiftstatusid",
                        column: x => x.workshiftstatusid,
                        principalTable: "workshiftstatus",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workshifttask",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    workshiftid = table.Column<Guid>(type: "uuid", nullable: false),
                    plannedstarttime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    actualstarttime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    actualendtime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    actualstartlatitude = table.Column<double>(type: "double precision", nullable: true),
                    actualstartlongitude = table.Column<double>(type: "double precision", nullable: true),
                    actualendlatitude = table.Column<double>(type: "double precision", nullable: true),
                    actualendlongitude = table.Column<double>(type: "double precision", nullable: true),
                    workshiftstatusid = table.Column<Guid>(type: "uuid", nullable: false),
                    note = table.Column<string>(type: "text", nullable: true),
                    createtime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    creator = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    edittime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    editor = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_workshifttask", x => x.id);
                    table.ForeignKey(
                        name: "fk_workshifttask_workshift_workshiftid",
                        column: x => x.workshiftid,
                        principalTable: "workshift",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_workshifttask_workshiftstatus_workshiftstatusid",
                        column: x => x.workshiftstatusid,
                        principalTable: "workshiftstatus",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_workshift_employeeid",
                table: "workshift",
                column: "employeeid");

            migrationBuilder.CreateIndex(
                name: "ix_workshift_workshiftstatusid",
                table: "workshift",
                column: "workshiftstatusid");

            migrationBuilder.CreateIndex(
                name: "ix_workshifttask_workshiftid",
                table: "workshifttask",
                column: "workshiftid");

            migrationBuilder.CreateIndex(
                name: "ix_workshifttask_workshiftstatusid",
                table: "workshifttask",
                column: "workshiftstatusid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "workshifttask");

            migrationBuilder.DropTable(
                name: "workshift");

            migrationBuilder.DropTable(
                name: "workshiftstatus");
        }
    }
}
