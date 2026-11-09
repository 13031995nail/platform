--liquibase formatted sql
--changeset ypolezhaev:2025-12-22-position endDelimiter:$$ splitStatements:false
DO
$$
BEGIN
    CREATE TABLE Position (
        primaryKey UUID NOT NULL,
        CreateTime TIMESTAMP(3) NULL,
        Creator VARCHAR(255) NULL,
        Editor VARCHAR(255) NULL,
        EditTime TIMESTAMP(3) NULL,
        Code VARCHAR(10) NULL,
        Name VARCHAR(256) NOT NULL,
        Actuality boolean NOT NULL,
        PRIMARY KEY (primaryKey)
    );

        IF NOT EXISTS (SELECT 1
                FROM storms
                WHERE primarykey IN ('1c5f6c4e-d153-4c7a-8e4f-6ec3f299734b')) THEN
                insert into storms (primarykey, name, type, isattribute, isoperation, isview, isclass, sharedoper, createtime,
                                        creator, edittime, editor, comment)
                values ('1c5f6c4e-d153-4c7a-8e4f-6ec3f299734b', 'Tmobis.Business.Common.Personnel.Position', null, false, false, false,
                        true, true, now() at time zone 'utc', null, null, null, null);
        END IF;

        IF NOT EXISTS (SELECT 1
                        FROM stormp
                        WHERE primarykey IN ('9a0ea2a5-2ff2-4170-8dba-f253a900c381')) THEN
                insert into stormp (primarykey, subject_m0, agent_m0, createtime, creator, edittime, editor)
                values ('9a0ea2a5-2ff2-4170-8dba-f253a900c381', '1c5f6c4e-d153-4c7a-8e4f-6ec3f299734b',
                                '426f7b77-9d6d-47fb-859d-12623b85ba07', now() at time zone 'utc', null, null, null);
        END IF;

        IF NOT EXISTS (SELECT 1
                        FROM stormp
                        WHERE primarykey IN ('3cab31d5-1b77-4cc1-95dc-28216ab90420')) THEN
                insert into stormp (primarykey, subject_m0, agent_m0, createtime, creator, edittime, editor)
                values ('3cab31d5-1b77-4cc1-95dc-28216ab90420', '1c5f6c4e-d153-4c7a-8e4f-6ec3f299734b',
                        '784ba5da-fe26-4872-8463-9f34fd6573b5', now() at time zone 'utc', null, null, null);
        END IF;

        IF NOT EXISTS (SELECT 1
                        FROM stormac
                        WHERE primarykey IN ('e17fea21-6375-43c0-8e89-177dc06de82c')) THEN
                insert into stormac (primarykey, typeaccess, filter_m0, permition_m0, createtime, creator, edittime, editor)
                values ('e17fea21-6375-43c0-8e89-177dc06de82c', 'Full', null, '9a0ea2a5-2ff2-4170-8dba-f253a900c381',
                        now() at time zone 'utc', null, null, null);
        END IF;

        IF NOT EXISTS (SELECT 1
                        FROM stormac
                        WHERE primarykey IN ('0e6315de-4b68-4757-813b-5d31d77f638c')) THEN
                insert into stormac (primarykey, typeaccess, filter_m0, permition_m0, createtime, creator, edittime, editor)
                values ('0e6315de-4b68-4757-813b-5d31d77f638c', 'Read', null, '3cab31d5-1b77-4cc1-95dc-28216ab90420',
                        now() at time zone 'utc', null, null, null);
        END IF;

END;
$$ LANGUAGE PLPGSQL;
