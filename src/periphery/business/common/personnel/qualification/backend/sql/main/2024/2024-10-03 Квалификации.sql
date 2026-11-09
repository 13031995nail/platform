--liquibase formatted sql
--changeset nvershinin:202410-22-qualification endDelimiter:$$ splitStatements:false
DO
$$
BEGIN
    CREATE TABLE Qualification (
        primaryKey UUID NOT NULL,
        Name VARCHAR(256) NOT NULL,
        QualificationGroupName VARCHAR(256) NOT NULL,
        Relevance VARCHAR(256) NOT NULL,
        PRIMARY KEY (primaryKey)
    );

    IF NOT EXISTS (
      SELECT 1
      FROM storms
      WHERE primarykey IN ('d407cf98-92e1-4282-b580-00f2dd971bb4')
    ) THEN
      insert into storms (primarykey, name, type, isattribute, isoperation, isview, isclass, sharedoper, createtime, creator, edittime, editor, comment) values
        ('d407cf98-92e1-4282-b580-00f2dd971bb4', 'Tmobis.Business.Common.Resources.Qualifications.Qualification', null, false, false, false, true, true, now() at time zone 'utc', null, null, null, null);
    END IF;

    IF NOT EXISTS (
      SELECT 1
      FROM stormp
      WHERE primarykey IN ('d407cf98-92e1-4282-b580-00f2dd971bb4')
    ) THEN
      insert into stormp (primarykey, subject_m0, agent_m0, createtime, creator, edittime, editor) values
        ('d407cf98-92e1-4282-b580-00f2dd971bb4', 'd407cf98-92e1-4282-b580-00f2dd971bb4', '784ba5da-fe26-4872-8463-9f34fd6573b5', now() at time zone 'utc', null, null, null);
    END IF;

    IF NOT EXISTS (
      SELECT 1
      FROM stormac
      WHERE primarykey IN ('d407cf98-92e1-4282-b580-00f2dd971bb4')
    ) THEN
      insert into stormac (primarykey, typeaccess, filter_m0, permition_m0, createtime, creator, edittime, editor) values
        ('d407cf98-92e1-4282-b580-00f2dd971bb4', 'Read', null, 'd407cf98-92e1-4282-b580-00f2dd971bb4', now() at time zone 'utc', null, null, null);
    END IF;

    IF NOT EXISTS (
      SELECT 1
      FROM stormac
      WHERE primarykey IN ('fe749dc2-a129-4f63-9bad-a2e56b61348e')
    ) THEN
      insert into stormac (primarykey, typeaccess, filter_m0, permition_m0, createtime, creator, edittime, editor) values
        ('fe749dc2-a129-4f63-9bad-a2e56b61348e', 'Full', null, 'fe749dc2-a129-4f63-9bad-a2e56b61348e', now() at time zone 'utc', null, null, null);
    END IF;

END;
$$ LANGUAGE PLPGSQL;
