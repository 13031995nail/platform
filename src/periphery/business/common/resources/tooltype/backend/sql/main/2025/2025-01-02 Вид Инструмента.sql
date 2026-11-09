--liquibase formatted sql
--changeset ypolezhaev:2025-01-02-tool-type endDelimiter:$$ splitStatements:false
DO
$$
BEGIN
    CREATE TABLE ToolType (
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

    IF NOT EXISTS (
      SELECT 1
      FROM storms
      WHERE primarykey IN ('8ab4deb0-5f2c-4240-9dee-d6abfe8d3b95')
    ) THEN
      insert into storms (primarykey, name, type, isattribute, isoperation, isview, isclass, sharedoper, createtime, creator, edittime, editor, comment) values
        ('8ab4deb0-5f2c-4240-9dee-d6abfe8d3b95', 'Tmobis.Business.Common.Resources.ToolType.ToolType', null, false, false, false, true, true, now() at time zone 'utc', null, null, null, null);
    END IF;

    IF NOT EXISTS (
      SELECT 1
      FROM stormp
      WHERE primarykey IN ('8ab4deb0-5f2c-4240-9dee-d6abfe8d3b95')
    ) THEN
      insert into stormp (primarykey, subject_m0, agent_m0, createtime, creator, edittime, editor) values
        ('8ab4deb0-5f2c-4240-9dee-d6abfe8d3b95', '8ab4deb0-5f2c-4240-9dee-d6abfe8d3b95', '784ba5da-fe26-4872-8463-9f34fd6573b5', now() at time zone 'utc', null, null, null);
    END IF;

    IF NOT EXISTS (
      SELECT 1
      FROM stormac
      WHERE primarykey IN ('8ab4deb0-5f2c-4240-9dee-d6abfe8d3b95')
    ) THEN
      insert into stormac (primarykey, typeaccess, filter_m0, permition_m0, createtime, creator, edittime, editor) values
        ('8ab4deb0-5f2c-4240-9dee-d6abfe8d3b95', 'Read', null, '8ab4deb0-5f2c-4240-9dee-d6abfe8d3b95', now() at time zone 'utc', null, null, null);
    END IF;

    IF NOT EXISTS (
      SELECT 1
      FROM stormac
      WHERE primarykey IN ('9e804016-50af-49f6-b49a-6aab568e5fe6')
    ) THEN
      insert into stormac (primarykey, typeaccess, filter_m0, permition_m0, createtime, creator, edittime, editor) values
        ('9e804016-50af-49f6-b49a-6aab568e5fe6', 'Full', null, '9e804016-50af-49f6-b49a-6aab568e5fe6', now() at time zone 'utc', null, null, null);
    END IF;

END;
$$ LANGUAGE PLPGSQL;
