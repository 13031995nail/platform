--liquibase formatted sql
--changeset ValiullinNR:alterstormsstormpstormacEmployee endDelimiter:$$ splitStatements:false

DO
$$
  BEGIN
    IF NOT EXISTS (SELECT 1
                   FROM storms
                   WHERE primarykey IN ('6f0a07b7-193c-43b1-8189-e133e7374596')) THEN
      insert into storms (primarykey, name, type, isattribute, isoperation, isview, isclass, sharedoper, createtime,
                          creator, edittime, editor, comment)
      values ('6f0a07b7-193c-43b1-8189-e133e7374596', 'Tmobis.Business.Core.Flexberry.Models.Employee', null, false, false, false,
              true, true, now() at time zone 'utc', null, null, null, null);
    END IF;

    IF NOT EXISTS (SELECT 1
                   FROM stormp
                   WHERE primarykey IN ('1992ab66-75ea-4e3f-8ca2-626effaa12b8')) THEN
      insert into stormp (primarykey, subject_m0, agent_m0, createtime, creator, edittime, editor)
      values ('1992ab66-75ea-4e3f-8ca2-626effaa12b8', '6f0a07b7-193c-43b1-8189-e133e7374596',
              '7d3f8cfa-83fc-4970-97ad-e9e0445965d6', now() at time zone 'utc', null, null, null);
    END IF;
	
	IF NOT EXISTS (SELECT 1
                   FROM stormp
                   WHERE primarykey IN ('483585be-07ff-46bb-818e-391b94c4f72f')) THEN
      insert into stormp (primarykey, subject_m0, agent_m0, createtime, creator, edittime, editor)
      values ('483585be-07ff-46bb-818e-391b94c4f72f', '6f0a07b7-193c-43b1-8189-e133e7374596',
              '426f7b77-9d6d-47fb-859d-12623b85ba07', now() at time zone 'utc', null, null, null);
    END IF;

    IF NOT EXISTS (SELECT 1
                   FROM stormac
                   WHERE primarykey IN ('ec55fcd8-9259-472f-8365-c80b38e6a6d2')) THEN
      insert into stormac (primarykey, typeaccess, filter_m0, permition_m0, createtime, creator, edittime, editor)
      values ('ec55fcd8-9259-472f-8365-c80b38e6a6d2', 'Full', null, '1992ab66-75ea-4e3f-8ca2-626effaa12b8',
              now() at time zone 'utc', null, null, null);
    END IF;
	IF NOT EXISTS (SELECT 1
                   FROM stormac
                   WHERE primarykey IN ('bfc27a54-9186-42d2-9e45-8c02e948daea')) THEN
      insert into stormac (primarykey, typeaccess, filter_m0, permition_m0, createtime, creator, edittime, editor)
      values ('bfc27a54-9186-42d2-9e45-8c02e948daea', 'Full', null, '483585be-07ff-46bb-818e-391b94c4f72f',
              now() at time zone 'utc', null, null, null);
    END IF;
	END;
$$ LANGUAGE PLPGSQL;
