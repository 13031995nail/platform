--liquibase formatted sql
--changeset ValiullinNR:alterstormsstormpstormacEmployeeSignature endDelimiter:$$ splitStatements:false

DO
$$
  BEGIN
    IF NOT EXISTS (SELECT 1
                   FROM storms
                   WHERE primarykey IN ('d35cc005-e7c0-4f31-a008-2af5570d93eb')) THEN
      insert into storms (primarykey, name, type, isattribute, isoperation, isview, isclass, sharedoper, createtime,
                          creator, edittime, editor, comment)
      values ('d35cc005-e7c0-4f31-a008-2af5570d93eb', 'Tmobis.Business.Process.Signs.EmployeeSignature', null, false, false, false,
              true, true, now() at time zone 'utc', null, null, null, null);
    END IF;

    IF NOT EXISTS (SELECT 1
                   FROM stormp
                   WHERE primarykey IN ('00e2df7f-2f2d-4823-bb00-cfa8e399ff21')) THEN
      insert into stormp (primarykey, subject_m0, agent_m0, createtime, creator, edittime, editor)
      values ('00e2df7f-2f2d-4823-bb00-cfa8e399ff21', 'd35cc005-e7c0-4f31-a008-2af5570d93eb',
              '7d3f8cfa-83fc-4970-97ad-e9e0445965d6', now() at time zone 'utc', null, null, null);
    END IF;
	
	IF NOT EXISTS (SELECT 1
                   FROM stormp
                   WHERE primarykey IN ('1beff6c5-ea20-4e7d-8698-2398cb66ff34')) THEN
      insert into stormp (primarykey, subject_m0, agent_m0, createtime, creator, edittime, editor)
      values ('1beff6c5-ea20-4e7d-8698-2398cb66ff34', 'd35cc005-e7c0-4f31-a008-2af5570d93eb',
              '426f7b77-9d6d-47fb-859d-12623b85ba07', now() at time zone 'utc', null, null, null);
    END IF;

    IF NOT EXISTS (SELECT 1
                   FROM stormac
                   WHERE primarykey IN ('27fe5059-195d-4e90-87b7-c8b07825a00a')) THEN
      insert into stormac (primarykey, typeaccess, filter_m0, permition_m0, createtime, creator, edittime, editor)
      values ('27fe5059-195d-4e90-87b7-c8b07825a00a', 'Full', null, '00e2df7f-2f2d-4823-bb00-cfa8e399ff21',
              now() at time zone 'utc', null, null, null);
    END IF;
	IF NOT EXISTS (SELECT 1
                   FROM stormac
                   WHERE primarykey IN ('2354afd3-3ccd-48a2-ba54-14c3d467d653')) THEN
      insert into stormac (primarykey, typeaccess, filter_m0, permition_m0, createtime, creator, edittime, editor)
      values ('2354afd3-3ccd-48a2-ba54-14c3d467d653', 'Full', null, '1beff6c5-ea20-4e7d-8698-2398cb66ff34',
              now() at time zone 'utc', null, null, null);
    END IF;
	END;
$$ LANGUAGE PLPGSQL;
