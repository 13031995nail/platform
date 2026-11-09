--liquibase formatted sql
--changeset nvershinin:202410-22-unit endDelimiter:$$ splitStatements:false
DO
$$
BEGIN
    CREATE TABLE Unit (
        primaryKey UUID NOT NULL,
        Name VARCHAR(256) NOT NULL,
        Relevance VARCHAR(256) NOT NULL,
        PRIMARY KEY (primaryKey)
    );

    IF NOT EXISTS (
      SELECT 1
      FROM storms
      WHERE primarykey IN ('abcb3022-51f4-4de7-bc8f-584e4d8dca37')
    ) THEN
      insert into storms (primarykey, name, type, isattribute, isoperation, isview, isclass, sharedoper, createtime, creator, edittime, editor, comment) values
        ('abcb3022-51f4-4de7-bc8f-584e4d8dca37', 'Tmobis.Business.Common.Resources.Unit.Unit', null, false, false, false, true, true, now() at time zone 'utc', null, null, null, null);
    END IF;

    IF NOT EXISTS (
      SELECT 1
      FROM stormp
      WHERE primarykey IN ('abcb3022-51f4-4de7-bc8f-584e4d8dca37')
    ) THEN
      insert into stormp (primarykey, subject_m0, agent_m0, createtime, creator, edittime, editor) values
        ('abcb3022-51f4-4de7-bc8f-584e4d8dca37', 'abcb3022-51f4-4de7-bc8f-584e4d8dca37', '784ba5da-fe26-4872-8463-9f34fd6573b5', now() at time zone 'utc', null, null, null);
    END IF;

    IF NOT EXISTS (
      SELECT 1
      FROM stormac
      WHERE primarykey IN ('abcb3022-51f4-4de7-bc8f-584e4d8dca37')
    ) THEN
      insert into stormac (primarykey, typeaccess, filter_m0, permition_m0, createtime, creator, edittime, editor) values
        ('abcb3022-51f4-4de7-bc8f-584e4d8dca37', 'Read', null, 'abcb3022-51f4-4de7-bc8f-584e4d8dca37', now() at time zone 'utc', null, null, null);
    END IF;

    IF NOT EXISTS (
      SELECT 1
      FROM stormac
      WHERE primarykey IN ('776b87d8-2428-4165-8a40-c2b923afa2a4')
    ) THEN
      insert into stormac (primarykey, typeaccess, filter_m0, permition_m0, createtime, creator, edittime, editor) values
        ('776b87d8-2428-4165-8a40-c2b923afa2a4', 'Full', null, '776b87d8-2428-4165-8a40-c2b923afa2a4', now() at time zone 'utc', null, null, null);
    END IF;

END;
$$ LANGUAGE PLPGSQL;