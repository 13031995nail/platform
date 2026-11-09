--liquibase formatted sql
--changeset ValiullinNR:alterstormsstormpstormacObjectFunction endDelimiter:$$ splitStatements:false

DO
$$
  BEGIN
    IF NOT EXISTS (SELECT 1
                   FROM storms
                   WHERE primarykey IN ('ab2ce30e-41d2-4385-8f4e-1078ce7e2d3e')) THEN
      insert into storms (primarykey, name, type, isattribute, isoperation, isview, isclass, sharedoper, createtime,
                          creator, edittime, editor, comment)
      values ('ab2ce30e-41d2-4385-8f4e-1078ce7e2d3e', 'Tmobis.Business.Common.Address.ObjectFunction', null, false, false, false,
              true, true, now() at time zone 'utc', null, null, null, null);
    END IF;

    IF NOT EXISTS (SELECT 1
                   FROM stormp
                   WHERE primarykey IN ('13cb5e1b-257c-4fdd-8500-920716c16f74')) THEN
      insert into stormp (primarykey, subject_m0, agent_m0, createtime, creator, edittime, editor)
      values ('13cb5e1b-257c-4fdd-8500-920716c16f74', 'ab2ce30e-41d2-4385-8f4e-1078ce7e2d3e',
              '7d3f8cfa-83fc-4970-97ad-e9e0445965d6', now() at time zone 'utc', null, null, null);
    END IF;
	
	IF NOT EXISTS (SELECT 1
                   FROM stormp
                   WHERE primarykey IN ('51f59547-98a0-41f8-97d9-7586bfe2a20c')) THEN
      insert into stormp (primarykey, subject_m0, agent_m0, createtime, creator, edittime, editor)
      values ('51f59547-98a0-41f8-97d9-7586bfe2a20c', 'ab2ce30e-41d2-4385-8f4e-1078ce7e2d3e',
              '426f7b77-9d6d-47fb-859d-12623b85ba07', now() at time zone 'utc', null, null, null);
    END IF;

    IF NOT EXISTS (SELECT 1
                   FROM stormac
                   WHERE primarykey IN ('900b6d55-a33a-4856-9b03-081e05ecc3c8')) THEN
      insert into stormac (primarykey, typeaccess, filter_m0, permition_m0, createtime, creator, edittime, editor)
      values ('900b6d55-a33a-4856-9b03-081e05ecc3c8', 'Full', null, '13cb5e1b-257c-4fdd-8500-920716c16f74',
              now() at time zone 'utc', null, null, null);
    END IF;
	IF NOT EXISTS (SELECT 1
                   FROM stormac
                   WHERE primarykey IN ('c28fb1e3-aeca-44e6-8094-ffdcfa0a7d24')) THEN
      insert into stormac (primarykey, typeaccess, filter_m0, permition_m0, createtime, creator, edittime, editor)
      values ('c28fb1e3-aeca-44e6-8094-ffdcfa0a7d24', 'Full', null, '51f59547-98a0-41f8-97d9-7586bfe2a20c',
              now() at time zone 'utc', null, null, null);
    END IF;
	END;
$$ LANGUAGE PLPGSQL;