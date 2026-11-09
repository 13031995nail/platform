--liquibase formatted sql
--changeset MenshchikovaKA:createRoleAllUsers endDelimiter:$$ splitStatements:false

DO
$$
BEGIN
	IF NOT EXISTS (SELECT 1
	           FROM stormag
	           WHERE primarykey IN ('f4266424-07a1-494b-a904-5a57f95dbdff')) THEN
	INSERT INTO STORMAG(primaryKey,  Name, IsUser, IsGroup, IsRole, Enabled, CreateTime, Creator, town)
	values ('f4266424-07a1-494b-a904-5a57f95dbdff', 'Все пользователи', false, false, true, true, NOW(), 'admin', '521fb267-b7c1-4169-b435-ae126b7938b8');
	END IF;

	IF NOT EXISTS (SELECT 1
	           FROM stormlr
	           WHERE primarykey IN ('7c55c9c1-5d46-4b0a-a817-73d429d23989')) THEN
	insert into stormlr (primarykey, agent_m0, role_m0, createtime, creator)
	values ('7c55c9c1-5d46-4b0a-a817-73d429d23989', '426f7b77-9d6d-47fb-859d-12623b85ba07', 'f4266424-07a1-494b-a904-5a57f95dbdff', now(), 'admin');
	END IF;

	IF NOT EXISTS (SELECT 1
	           FROM stormlr
	           WHERE primarykey IN ('6f719047-12cc-4e7c-8bb3-964a60ccf1b8')) THEN
	insert into stormlr (primarykey, agent_m0, role_m0, createtime, creator)
	values ('6f719047-12cc-4e7c-8bb3-964a60ccf1b8', '9f54a379-1bc3-4f34-95a3-ec9e1a76f2f3', 'f4266424-07a1-494b-a904-5a57f95dbdff', now(), 'admin');
	END IF;

	IF NOT EXISTS (SELECT 1
	           FROM stormlr
	           WHERE primarykey IN ('9bc32c2d-abb2-48ed-a0bb-40307aedb554')) THEN
	insert into stormlr (primarykey, agent_m0, role_m0, createtime, creator)
	values ('9bc32c2d-abb2-48ed-a0bb-40307aedb554', '7d3f8cfa-83fc-4970-97ad-e9e0445965d6', 'f4266424-07a1-494b-a904-5a57f95dbdff', now(), 'admin');
	END IF;

	IF NOT EXISTS (SELECT 1
	           FROM stormlr
	           WHERE primarykey IN ('bf40e2a1-bd2a-4707-a6b0-b8e2c3fb3654')) THEN
	insert into stormlr (primarykey, agent_m0, role_m0, createtime, creator)
	values ('bf40e2a1-bd2a-4707-a6b0-b8e2c3fb3654', '784ba5da-fe26-4872-8463-9f34fd6573b5', 'f4266424-07a1-494b-a904-5a57f95dbdff', now(), 'admin');
	END IF;

	IF NOT EXISTS (SELECT 1
	           FROM stormlr
	           WHERE primarykey IN ('d634b8fd-3410-453b-aa16-022f93b32d1f')) THEN
	insert into stormlr (primarykey, agent_m0, role_m0, createtime, creator)
	values ('d634b8fd-3410-453b-aa16-022f93b32d1f', 'a1b2c3d4-e5f6-7890-abcd-ef1234567890', 'f4266424-07a1-494b-a904-5a57f95dbdff', now(), 'admin');
	END IF;

	IF NOT EXISTS (SELECT 1
	           FROM stormlr
	           WHERE primarykey IN ('70e39aac-b9d0-4b7a-bc3c-c63ed14717ce')) THEN
	insert into stormlr (primarykey, agent_m0, role_m0, createtime, creator)
	values ('70e39aac-b9d0-4b7a-bc3c-c63ed14717ce', '00d82ecd-83a8-4622-a8a2-11bbaeb3b29b', 'f4266424-07a1-494b-a904-5a57f95dbdff', now(), 'admin');
	END IF;

	IF NOT EXISTS (SELECT 1
	           FROM stormlr
	           WHERE primarykey IN ('7f36a1a9-70f5-4ca2-a7d0-b4006aaa5747')) THEN
	insert into stormlr (primarykey, agent_m0, role_m0, createtime, creator)
	values ('7f36a1a9-70f5-4ca2-a7d0-b4006aaa5747', 'cebc7c4f-6110-44d2-b45f-3ff8566a619e', 'f4266424-07a1-494b-a904-5a57f95dbdff', now(), 'admin');
	END IF;

END;
$$ LANGUAGE PLPGSQL;

