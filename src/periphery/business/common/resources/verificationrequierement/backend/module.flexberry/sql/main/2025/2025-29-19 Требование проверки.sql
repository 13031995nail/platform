--liquibase formatted sql
--changeset ypolezhaev:2025-01-02-verification-requierement endDelimiter:$$ splitStatements:false
DO
$$
BEGIN
    CREATE TABLE VerificationRequierement (
        primaryKey UUID NOT NULL,
        CreateTime TIMESTAMP(3) NULL,
        Creator VARCHAR(255) NULL,
        Editor VARCHAR(255) NULL,
        EditTime TIMESTAMP(3) NULL,
        Code VARCHAR(10) NULL,
        Name VARCHAR(256) NOT NULL,
        Actuality boolean NOT NULL,
        IsCalculated boolean NOT NULL,
        ReqNumber VARCHAR(10) NOT NULL,
        ShortIndicatorName VARCHAR(10) NULL,
        ShortReqName VARCHAR(250) NULL,
        IndicatorName VARCHAR(250) NOT NULL,
        SupportingDocName VARCHAR(10) NULL,
        Weight DECIMAL(15, 2) NULL,
        OrganizationType UUID NOT NULL,
        PRIMARY KEY (primaryKey)
    );

    ALTER TABLE VerificationRequierement ADD CONSTRAINT FK_VerificationRequierement_OrganizationType FOREIGN KEY (OrganizationType) REFERENCES OrganizationType;
    CREATE INDEX Index1d760aa4737e5440c657a2cb3a24522313266c40 on VerificationRequierement (OrganizationType);

        IF NOT EXISTS (SELECT 1
                FROM storms
                WHERE primarykey IN ('913c0957-f4e2-4d0a-a0c7-018b3914b916')) THEN
                insert into storms (primarykey, name, type, isattribute, isoperation, isview, isclass, sharedoper, createtime,
                                        creator, edittime, editor, comment)
                values ('913c0957-f4e2-4d0a-a0c7-018b3914b916', 'Tmobis.Business.Common.Resources.VerificationRequierement', null, false, false, false,
                        true, true, now() at time zone 'utc', null, null, null, null);
        END IF;

        IF NOT EXISTS (SELECT 1
                        FROM stormp
                        WHERE primarykey IN ('b0ff5e6c-bfd5-4372-bb68-a6bcfe5e04e9')) THEN
                insert into stormp (primarykey, subject_m0, agent_m0, createtime, creator, edittime, editor)
                values ('b0ff5e6c-bfd5-4372-bb68-a6bcfe5e04e9', '913c0957-f4e2-4d0a-a0c7-018b3914b916',
                                '426f7b77-9d6d-47fb-859d-12623b85ba07', now() at time zone 'utc', null, null, null);
        END IF;

        IF NOT EXISTS (SELECT 1
                        FROM stormp
                        WHERE primarykey IN ('cc2f43bc-7933-4397-8fcb-664b9bc310c4')) THEN
                insert into stormp (primarykey, subject_m0, agent_m0, createtime, creator, edittime, editor)
                values ('cc2f43bc-7933-4397-8fcb-664b9bc310c4', '913c0957-f4e2-4d0a-a0c7-018b3914b916',
                        '784ba5da-fe26-4872-8463-9f34fd6573b5', now() at time zone 'utc', null, null, null);
        END IF;

        IF NOT EXISTS (SELECT 1
                        FROM stormac
                        WHERE primarykey IN ('6eb1240e-dab6-48f3-9514-aae5422b2d91')) THEN
                insert into stormac (primarykey, typeaccess, filter_m0, permition_m0, createtime, creator, edittime, editor)
                values ('6eb1240e-dab6-48f3-9514-aae5422b2d91', 'Full', null, 'b0ff5e6c-bfd5-4372-bb68-a6bcfe5e04e9',
                        now() at time zone 'utc', null, null, null);
        END IF;

        IF NOT EXISTS (SELECT 1
                        FROM stormac
                        WHERE primarykey IN ('4872b841-9b02-432f-98de-d4e6381264b6')) THEN
                insert into stormac (primarykey, typeaccess, filter_m0, permition_m0, createtime, creator, edittime, editor)
                values ('4872b841-9b02-432f-98de-d4e6381264b6', 'Read', null, 'cc2f43bc-7933-4397-8fcb-664b9bc310c4',
                        now() at time zone 'utc', null, null, null);
        END IF;

END;
$$ LANGUAGE PLPGSQL;
