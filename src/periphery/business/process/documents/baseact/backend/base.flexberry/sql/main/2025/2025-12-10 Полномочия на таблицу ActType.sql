--liquibase formatted sql
--changeset ValiullinNR:alterstormsstormpstormacActType endDelimiter:$$ splitStatements:false

DO
$$
  BEGIN
    IF NOT EXISTS (SELECT 1
                   FROM storms
                   WHERE primarykey IN ('8ca156b4-2766-430d-bdb6-21499b6be76b')) THEN
      insert into storms (primarykey, name, type, isattribute, isoperation, isview, isclass, sharedoper, createtime,
                          creator, edittime, editor, comment)
      values ('8ca156b4-2766-430d-bdb6-21499b6be76b', 'Tmobis.Business.Process.Documents.ActType', null, false, false, false,
              true, true, now() at time zone 'utc', null, null, null, null);
    END IF;

    IF NOT EXISTS (SELECT 1
                   FROM stormp
                   WHERE primarykey IN ('ba514588-09f3-415f-9b8b-696405593c16')) THEN
      insert into stormp (primarykey, subject_m0, agent_m0, createtime, creator, edittime, editor)
      values ('ba514588-09f3-415f-9b8b-696405593c16', '8ca156b4-2766-430d-bdb6-21499b6be76b',
              '7d3f8cfa-83fc-4970-97ad-e9e0445965d6', now() at time zone 'utc', null, null, null);
    END IF;
	
	IF NOT EXISTS (SELECT 1
                   FROM stormp
                   WHERE primarykey IN ('9237dea4-bba0-404d-848b-8d3f811e72cd')) THEN
      insert into stormp (primarykey, subject_m0, agent_m0, createtime, creator, edittime, editor)
      values ('9237dea4-bba0-404d-848b-8d3f811e72cd', '8ca156b4-2766-430d-bdb6-21499b6be76b',
              '426f7b77-9d6d-47fb-859d-12623b85ba07', now() at time zone 'utc', null, null, null);
    END IF;

    IF NOT EXISTS (SELECT 1
                   FROM stormac
                   WHERE primarykey IN ('cee074aa-6ff8-444f-9541-bb516c629cff')) THEN
      insert into stormac (primarykey, typeaccess, filter_m0, permition_m0, createtime, creator, edittime, editor)
      values ('cee074aa-6ff8-444f-9541-bb516c629cff', 'Full', null, 'ba514588-09f3-415f-9b8b-696405593c16',
              now() at time zone 'utc', null, null, null);
    END IF;
	
	IF NOT EXISTS (SELECT 1
                   FROM stormac
                   WHERE primarykey IN ('45a644e9-dd23-4de8-ab9b-d5575678aa67')) THEN
      insert into stormac (primarykey, typeaccess, filter_m0, permition_m0, createtime, creator, edittime, editor)
      values ('45a644e9-dd23-4de8-ab9b-d5575678aa67', 'Full', null, '9237dea4-bba0-404d-848b-8d3f811e72cd',
              now() at time zone 'utc', null, null, null);
    END IF;
    END;
$$ LANGUAGE PLPGSQL;
