namespace Tmobis.Objects.EF.Migrations;

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Tmobis.Data.EF.Contexts;

[DbContext(typeof(DatabaseContext))]
[Migration("20260826090000_AddMattermostNotificationProvider")]
public partial class AddMattermostNotificationProvider : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            INSERT INTO notificationprovidertype (id, actuality, code, name)
            VALUES ('6885204c-65b4-4deb-be9f-b95d4c69f2f7', TRUE, 'mattermost', 'Mattermost');
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            DELETE FROM notificationprovidertype
            WHERE id = '6885204c-65b4-4deb-be9f-b95d4c69f2f7';
            """);
    }
}
