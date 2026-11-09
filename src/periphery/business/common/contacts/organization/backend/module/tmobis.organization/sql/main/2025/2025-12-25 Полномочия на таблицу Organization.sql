--liquibase formatted sql
--changeset ValiullinNR:alterstormsstormpstormacOrganization endDelimiter:$$ splitStatements:false

DO
$$
  BEGIN
    IF NOT EXISTS (SELECT 1
                   FROM storms
                   WHERE primarykey IN ('c1d0f6a3-4a17-405b-bb4d-ad45609863aa')) THEN
      insert into storms (primarykey, name, type, isattribute, isoperation, isview, isclass, sharedoper, createtime,
                          creator, edittime, editor, comment)
      values ('c1d0f6a3-4a17-405b-bb4d-ad45609863aa', 'Tmobis.Business.Common.Contacts.Organization', null, false, false, false,
              true, true, now() at time zone 'utc', null, null, null, null);
    END IF;

    IF NOT EXISTS (SELECT 1
                   FROM stormp
                   WHERE primarykey IN ('26dff234-16fe-4025-9033-696dd209a05e')) THEN
      insert into stormp (primarykey, subject_m0, agent_m0, createtime, creator, edittime, editor)
      values ('26dff234-16fe-4025-9033-696dd209a05e', 'c1d0f6a3-4a17-405b-bb4d-ad45609863aa',
              '7d3f8cfa-83fc-4970-97ad-e9e0445965d6', now() at time zone 'utc', null, null, null);
    END IF;
	
	IF NOT EXISTS (SELECT 1
                   FROM stormp
                   WHERE primarykey IN ('61c542a7-06c7-4250-8424-46547b586340')) THEN
      insert into stormp (primarykey, subject_m0, agent_m0, createtime, creator, edittime, editor)
      values ('61c542a7-06c7-4250-8424-46547b586340', 'c1d0f6a3-4a17-405b-bb4d-ad45609863aa',
              '426f7b77-9d6d-47fb-859d-12623b85ba07', now() at time zone 'utc', null, null, null);
    END IF;

    IF NOT EXISTS (SELECT 1
                   FROM stormac
                   WHERE primarykey IN ('2c396105-9717-4fe5-b1f2-a99650792d61')) THEN
      insert into stormac (primarykey, typeaccess, filter_m0, permition_m0, createtime, creator, edittime, editor)
      values ('2c396105-9717-4fe5-b1f2-a99650792d61', 'Full', null, '26dff234-16fe-4025-9033-696dd209a05e',
              now() at time zone 'utc', null, null, null);
    END IF;
	IF NOT EXISTS (SELECT 1
                   FROM stormac
                   WHERE primarykey IN ('01a684b6-f501-4456-b5f5-1ea5d41e1a10')) THEN
      insert into stormac (primarykey, typeaccess, filter_m0, permition_m0, createtime, creator, edittime, editor)
      values ('01a684b6-f501-4456-b5f5-1ea5d41e1a10', 'Full', null, '61c542a7-06c7-4250-8424-46547b586340',
              now() at time zone 'utc', null, null, null);
    END IF;
	END;
$$ LANGUAGE PLPGSQL;