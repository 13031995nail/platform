namespace Tmobis.Objects.EF.Migrations;

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Tmobis.Data.EF.Contexts;

[DbContext(typeof(DatabaseContext))]
[Migration("20260904120000_UseMessengerDirectory")]
public partial class UseMessengerDirectory : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            CREATE TABLE messenger (
                id uuid NOT NULL,
                createtime timestamp with time zone NULL,
                creator character varying(255) NULL,
                edittime timestamp with time zone NULL,
                editor character varying(255) NULL,
                actuality boolean NOT NULL,
                code character varying(50) NULL,
                name character varying(100) NOT NULL,
                CONSTRAINT pk_messenger PRIMARY KEY (id)
            );

            INSERT INTO messenger (id, actuality, code, name) VALUES
                ('7e4d1688-dbc0-4267-8c5e-0f46140caea9', TRUE, 'max', 'MAX'),
                ('7c289438-7257-4d5d-8c55-707970d3ee84', TRUE, 'telegram', 'Telegram'),
                ('60553b17-e401-48ca-8e39-d9bdbf80ad4c', TRUE, 'vk', 'VK'),
                ('a7f97578-6df8-4830-ae04-99bf7f8db261', TRUE, 'mattermost', 'Mattermost');

            CREATE UNIQUE INDEX ix_messenger_code ON messenger (code);

            DROP INDEX IF EXISTS ix_employeemessenger_employeeid_messenger;
            DROP INDEX IF EXISTS ix_employeemessenger_messenger_messengerid;

            ALTER TABLE employeemessenger
                RENAME COLUMN messengerid TO externalid;
            ALTER TABLE employeemessenger
                ADD COLUMN messengerid uuid NULL;
            ALTER TABLE employeemessenger
                ADD COLUMN ispreferred boolean NOT NULL DEFAULT FALSE;

            UPDATE employeemessenger
            SET messengerid = CASE messenger
                WHEN 1 THEN '7e4d1688-dbc0-4267-8c5e-0f46140caea9'::uuid
                WHEN 2 THEN '7c289438-7257-4d5d-8c55-707970d3ee84'::uuid
                WHEN 3 THEN '60553b17-e401-48ca-8e39-d9bdbf80ad4c'::uuid
                WHEN 4 THEN 'a7f97578-6df8-4830-ae04-99bf7f8db261'::uuid
            END;

            INSERT INTO employeemessenger (
                id,
                employeeid,
                messenger,
                externalid,
                messengerid,
                ispreferred)
            SELECT
                md5(employee.id::text || ':max')::uuid,
                employee.id,
                1,
                employee.maxchatid::text,
                '7e4d1688-dbc0-4267-8c5e-0f46140caea9'::uuid,
                FALSE
            FROM employee
            WHERE employee.maxchatid IS NOT NULL
                AND NOT EXISTS (
                    SELECT 1
                    FROM employeemessenger AS link
                    WHERE link.employeeid = employee.id
                        AND link.messengerid =
                            '7e4d1688-dbc0-4267-8c5e-0f46140caea9'::uuid
                );

            UPDATE employeemessenger AS link
            SET ispreferred = TRUE
            FROM employee, "user"
            WHERE link.employeeid = employee.id
                AND employee.userid = "user".id
                AND link.messenger = "user".preferredmessenger;

            DO $$
            BEGIN
                IF EXISTS (
                    SELECT 1
                    FROM employeemessenger
                    WHERE messengerid IS NULL
                ) THEN
                    RAISE EXCEPTION 'Unknown messenger value in employeemessenger';
                END IF;
            END $$;

            ALTER TABLE employeemessenger
                ALTER COLUMN messengerid SET NOT NULL;
            ALTER TABLE employeemessenger
                DROP COLUMN messenger;
            ALTER TABLE "user"
                DROP COLUMN preferredmessenger;
            ALTER TABLE employee
                DROP COLUMN maxchatid;

            ALTER TABLE employeemessenger
                ADD CONSTRAINT fk_employeemessenger_messenger_messengerid
                FOREIGN KEY (messengerid) REFERENCES messenger (id)
                ON DELETE CASCADE;

            CREATE INDEX ix_employeemessenger_messengerid
                ON employeemessenger (messengerid);
            CREATE UNIQUE INDEX ix_employeemessenger_employeeid_messengerid
                ON employeemessenger (employeeid, messengerid);
            CREATE UNIQUE INDEX ix_employeemessenger_messengerid_externalid
                ON employeemessenger (messengerid, externalid);
            CREATE UNIQUE INDEX ix_employeemessenger_preferred_employee
                ON employeemessenger (employeeid)
                WHERE ispreferred;
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            ALTER TABLE "user"
                ADD COLUMN preferredmessenger integer NULL;
            ALTER TABLE employee
                ADD COLUMN maxchatid bigint NULL;
            ALTER TABLE employeemessenger
                ADD COLUMN messenger integer NULL;

            UPDATE employeemessenger AS link
            SET messenger = CASE messenger.code
                WHEN 'max' THEN 1
                WHEN 'telegram' THEN 2
                WHEN 'vk' THEN 3
                WHEN 'mattermost' THEN 4
            END
            FROM messenger
            WHERE link.messengerid = messenger.id;

            UPDATE "user"
            SET preferredmessenger = link.messenger
            FROM employee, employeemessenger AS link
            WHERE employee.userid = "user".id
                AND link.employeeid = employee.id
                AND link.ispreferred;

            UPDATE employee
            SET maxchatid = link.externalid::bigint
            FROM employeemessenger AS link
            WHERE link.employeeid = employee.id
                AND link.messenger = 1
                AND link.externalid ~ '^[0-9]+$';

            DROP INDEX ix_employeemessenger_preferred_employee;
            DROP INDEX ix_employeemessenger_messengerid_externalid;
            DROP INDEX ix_employeemessenger_employeeid_messengerid;
            DROP INDEX ix_employeemessenger_messengerid;
            ALTER TABLE employeemessenger
                DROP CONSTRAINT fk_employeemessenger_messenger_messengerid;

            ALTER TABLE employeemessenger
                DROP COLUMN messengerid;
            ALTER TABLE employeemessenger
                DROP COLUMN ispreferred;
            ALTER TABLE employeemessenger
                RENAME COLUMN externalid TO messengerid;
            ALTER TABLE employeemessenger
                ALTER COLUMN messenger SET NOT NULL;

            CREATE UNIQUE INDEX ix_employeemessenger_employeeid_messenger
                ON employeemessenger (employeeid, messenger);
            CREATE UNIQUE INDEX ix_employeemessenger_messenger_messengerid
                ON employeemessenger (messenger, messengerid);

            DROP TABLE messenger;
            """);
    }
}
