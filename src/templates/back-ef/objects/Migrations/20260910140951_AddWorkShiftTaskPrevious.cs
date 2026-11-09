using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tmobis.Objects.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkShiftTaskPrevious : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "previousworkshifttaskid",
                table: "workshifttask",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_workshifttask_previousworkshifttaskid",
                table: "workshifttask",
                column: "previousworkshifttaskid");

            migrationBuilder.AddForeignKey(
                name: "fk_workshifttask_workshifttask_previousworkshifttaskid",
                table: "workshifttask",
                column: "previousworkshifttaskid",
                principalTable: "workshifttask",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_workshifttask_workshifttask_previousworkshifttaskid",
                table: "workshifttask");

            migrationBuilder.DropIndex(
                name: "ix_workshifttask_previousworkshifttaskid",
                table: "workshifttask");

            migrationBuilder.DropColumn(
                name: "previousworkshifttaskid",
                table: "workshifttask");
        }
    }
}
