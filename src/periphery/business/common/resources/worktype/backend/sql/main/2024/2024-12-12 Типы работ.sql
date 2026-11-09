--liquibase formatted sql
--changeset apachin:2024-12-18-work-type
DO
$$
BEGIN
    CREATE TABLE WorkType (
        primaryKey UUID NOT NULL,
        Name VARCHAR(250) NOT NULL,
        Actuality boolean NOT NULL,
        PRIMARY KEY (primaryKey)
    );

    IF NOT EXISTS (
      SELECT 1
      FROM storms
      WHERE primarykey IN ('8ab4deb0-5f2c-4240-9dee-d6abfe8d3b95')
    ) THEN
      insert into storms (primarykey, name, type, isattribute, isoperation, isview, isclass, sharedoper, createtime, creator, edittime, editor, comment) values
        ('8ab4deb0-5f2c-4240-9dee-d6abfe8d3b95', 'Tmobis.Business.Common.Resources.WorkType.WorkType', null, false, false, false, true, true, now() at time zone 'utc', null, null, null, null);
    END IF;

    IF NOT EXISTS (
      SELECT 1
      FROM stormp
      WHERE primarykey IN ('93da194b-b228-45b4-8e64-7d4a16b9b88b',
        'f2708a4b-1efb-4e37-8f7e-554eda460e82')
    ) THEN
      insert into stormp (primarykey, subject_m0, agent_m0, createtime, creator, edittime, editor) values
        ('93da194b-b228-45b4-8e64-7d4a16b9b88b', '8ab4deb0-5f2c-4240-9dee-d6abfe8d3b95', '784ba5da-fe26-4872-8463-9f34fd6573b5', now() at time zone 'utc', null, null, null),
        ('f2708a4b-1efb-4e37-8f7e-554eda460e82', '8ab4deb0-5f2c-4240-9dee-d6abfe8d3b95', '426f7b77-9d6d-47fb-859d-12623b85ba07', now() at time zone 'utc', null, null, null);
    END IF;

    IF NOT EXISTS (
      SELECT 1
      FROM stormac
      WHERE primarykey IN ('901f6e27-369e-4f7f-9354-22121fe95316',
        '9e804016-50af-49f6-b49a-6aab568e5fe6')
    ) THEN
      insert into stormac (primarykey, typeaccess, filter_m0, permition_m0, createtime, creator, edittime, editor) values
        ('901f6e27-369e-4f7f-9354-22121fe95316', 'Read', null, '93da194b-b228-45b4-8e64-7d4a16b9b88b', now() at time zone 'utc', null, null, null);
        ('9e804016-50af-49f6-b49a-6aab568e5fe6', 'Full', null, 'f2708a4b-1efb-4e37-8f7e-554eda460e82', now() at time zone 'utc', null, null, null);
    END IF;

END;
$$ LANGUAGE PLPGSQL;
