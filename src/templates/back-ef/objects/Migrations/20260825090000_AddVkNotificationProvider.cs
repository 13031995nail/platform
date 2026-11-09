namespace Tmobis.Objects.EF.Migrations;

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Tmobis.Data.EF.Contexts;

[DbContext(typeof(DatabaseContext))]
[Migration("20260825090000_AddVkNotificationProvider")]
public partial class AddVkNotificationProvider : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            INSERT INTO notificationprovidertype (id, actuality, code, name)
            VALUES ('c5467a74-bca7-4d86-a53a-0b8174252580', TRUE, 'vk', 'VK');
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            DELETE FROM notificationprovidertype
            WHERE id = 'c5467a74-bca7-4d86-a53a-0b8174252580';
            """);
    }
}
