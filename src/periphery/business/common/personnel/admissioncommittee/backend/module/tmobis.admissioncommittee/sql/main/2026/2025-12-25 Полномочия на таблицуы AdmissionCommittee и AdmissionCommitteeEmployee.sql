--liquibase formatted sql
--changeset ValiullinNR:alterstormsstormpstormacAdmissionCommittee endDelimiter:$$ splitStatements:false

DO
$$
  BEGIN
    IF NOT EXISTS (SELECT 1
                   FROM storms
                   WHERE primarykey IN ('5cc68927-5ea8-4f26-8d13-649f4e01b58c',
										'a06bc6f6-d94c-486e-a5c6-ab82786f81b8')) THEN
      insert into storms (primarykey, name, type, isattribute, isoperation, isview, isclass, sharedoper, createtime,
                          creator, edittime, editor, comment)
      values ('5cc68927-5ea8-4f26-8d13-649f4e01b58c', 'Tmobis.Business.Common.Personnel.AdmissionCommittee', null, false, false, false,
              true, true, now() at time zone 'utc', null, null, null, null),
			 ('a06bc6f6-d94c-486e-a5c6-ab82786f81b8', 'Tmobis.Business.Common.Personnel.AdmissionCommitteeEmployee', null, false, false, false,
              true, true, now() at time zone 'utc', null, null, null, null);
    END IF;

    IF NOT EXISTS (SELECT 1
                   FROM stormp
                   WHERE primarykey IN ('ad553f52-a3fa-406b-a6c7-89b7be6893a6',
										'59315251-8853-443d-bdc9-842bf7535eef',
										'235c5fee-793d-4c31-b946-f7ffc98ccea7',
										'0f5ffab8-d438-4ae8-8c12-8d172cffc2f6')) THEN
      insert into stormp (primarykey, subject_m0, agent_m0, createtime, creator, edittime, editor)
      values ('ad553f52-a3fa-406b-a6c7-89b7be6893a6', '5cc68927-5ea8-4f26-8d13-649f4e01b58c',
              '7d3f8cfa-83fc-4970-97ad-e9e0445965d6', now() at time zone 'utc', null, null, null),
			 ('59315251-8853-443d-bdc9-842bf7535eef', '5cc68927-5ea8-4f26-8d13-649f4e01b58c',
              '426f7b77-9d6d-47fb-859d-12623b85ba07', now() at time zone 'utc', null, null, null),		  
			 ('235c5fee-793d-4c31-b946-f7ffc98ccea7', 'a06bc6f6-d94c-486e-a5c6-ab82786f81b8',
              '7d3f8cfa-83fc-4970-97ad-e9e0445965d6', now() at time zone 'utc', null, null, null),
			 ('0f5ffab8-d438-4ae8-8c12-8d172cffc2f6', 'a06bc6f6-d94c-486e-a5c6-ab82786f81b8',
              '426f7b77-9d6d-47fb-859d-12623b85ba07', now() at time zone 'utc', null, null, null);
    END IF;

    IF NOT EXISTS (SELECT 1
                   FROM stormac
                   WHERE primarykey IN ('ef645730-6750-4846-8ddf-a6cfb4eb1a41',
										'fbbefe08-9aa4-4333-b727-4ca6c8607dc0',
										'11abda00-1282-422e-8f9f-dc8741af9235',
										'7cf02dac-c8b7-48c5-9ed9-f447f222d1f9')) THEN
      insert into stormac (primarykey, typeaccess, filter_m0, permition_m0, createtime, creator, edittime, editor)
      values ('ef645730-6750-4846-8ddf-a6cfb4eb1a41', 'Full', null, 'ad553f52-a3fa-406b-a6c7-89b7be6893a6',
              now() at time zone 'utc', null, null, null),
			 ('fbbefe08-9aa4-4333-b727-4ca6c8607dc0', 'Full', null, '59315251-8853-443d-bdc9-842bf7535eef',
              now() at time zone 'utc', null, null, null),
			 ('11abda00-1282-422e-8f9f-dc8741af9235', 'Full', null, '235c5fee-793d-4c31-b946-f7ffc98ccea7',
              now() at time zone 'utc', null, null, null),
			 ('7cf02dac-c8b7-48c5-9ed9-f447f222d1f9', 'Full', null, '0f5ffab8-d438-4ae8-8c12-8d172cffc2f6',
              now() at time zone 'utc', null, null, null);
    END IF;
	END;
$$ LANGUAGE PLPGSQL;
