--liquibase formatted sql
--changeset ValiullinNR:alterstormsstormpstormacWorkObject endDelimiter:$$ splitStatements:false

DO
$$
  BEGIN
    IF NOT EXISTS (SELECT 1
                   FROM storms
                   WHERE primarykey IN ('34f2c24c-bc27-4e90-b8bd-d4cd5e464363')) THEN
      insert into storms (primarykey, name, type, isattribute, isoperation, isview, isclass, sharedoper, createtime,
                          creator, edittime, editor, comment)
      values ('34f2c24c-bc27-4e90-b8bd-d4cd5e464363', 'Tmobis.Business.Common.Address.WorkObject', null, false, false, false,
              true, true, now() at time zone 'utc', null, null, null, null);
    END IF;

    IF NOT EXISTS (SELECT 1
                   FROM stormp
                   WHERE primarykey IN ('bf48a808-cb6b-45e8-be27-7e0198d2b5be')) THEN
      insert into stormp (primarykey, subject_m0, agent_m0, createtime, creator, edittime, editor)
      values ('bf48a808-cb6b-45e8-be27-7e0198d2b5be', '34f2c24c-bc27-4e90-b8bd-d4cd5e464363',
              '7d3f8cfa-83fc-4970-97ad-e9e0445965d6', now() at time zone 'utc', null, null, null);
    END IF;
	
	IF NOT EXISTS (SELECT 1
                   FROM stormp
                   WHERE primarykey IN ('105cc83b-3fe3-4988-9a11-d3e091a99b00')) THEN
      insert into stormp (primarykey, subject_m0, agent_m0, createtime, creator, edittime, editor)
      values ('105cc83b-3fe3-4988-9a11-d3e091a99b00', '34f2c24c-bc27-4e90-b8bd-d4cd5e464363',
              '426f7b77-9d6d-47fb-859d-12623b85ba07', now() at time zone 'utc', null, null, null);
    END IF;

    IF NOT EXISTS (SELECT 1
                   FROM stormac
                   WHERE primarykey IN ('ed062397-cade-4faf-bef2-34163b552433')) THEN
      insert into stormac (primarykey, typeaccess, filter_m0, permition_m0, createtime, creator, edittime, editor)
      values ('ed062397-cade-4faf-bef2-34163b552433', 'Full', null, 'bf48a808-cb6b-45e8-be27-7e0198d2b5be',
              now() at time zone 'utc', null, null, null);
    END IF;
	IF NOT EXISTS (SELECT 1
                   FROM stormac
                   WHERE primarykey IN ('8973f05b-a84c-491b-9e1c-c0d5a8432b88')) THEN
      insert into stormac (primarykey, typeaccess, filter_m0, permition_m0, createtime, creator, edittime, editor)
      values ('8973f05b-a84c-491b-9e1c-c0d5a8432b88', 'Full', null, '105cc83b-3fe3-4988-9a11-d3e091a99b00',
              now() at time zone 'utc', null, null, null);
    END IF;
	END;
$$ LANGUAGE PLPGSQL;
