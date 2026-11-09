--liquibase formatted sql
--changeset GlukhovMN:createRoleUserAdmin endDelimiter:$$ splitStatements:false

DO
$$
BEGIN
	IF EXISTS (SELECT 1
	           FROM stormac
	           WHERE primarykey IN ('019274fa-b78c-7e76-a8a3-9ff071c80d34')) THEN
    delete from stormac where primarykey = '019274fa-b78c-7e76-a8a3-9ff071c80d34';
	insert into stormac (primarykey, typeaccess, filter_m0, permition_m0, createtime, creator, edittime, editor)
	values ('295a0754-708e-4497-a7d8-1c9f802de1c6', 'Read', null, '019274fa-b78c-769c-bc2d-3ab94aefd6dc', now() at time zone 'utc', null, null, null),
    ('1f618b79-ce76-40f5-8574-d505c2d1be8f', 'Update', null, '019274fa-b78c-769c-bc2d-3ab94aefd6dc', now() at time zone 'utc', null, null, null),
    ('cf7e70ed-f133-4eae-bf1d-ee8ab36998ab', 'Delete', null, '019274fa-b78c-769c-bc2d-3ab94aefd6dc', now() at time zone 'utc', null, null, null),
    ('c4826477-b167-47b7-94d5-126eb0eb3cea', 'Execute', null, '019274fa-b78c-769c-bc2d-3ab94aefd6dc', now() at time zone 'utc', null, null, null)
	ON CONFLICT (primarykey) DO NOTHING;
	END IF;

	IF EXISTS (SELECT 1
	           FROM stormac
	           WHERE primarykey IN ('561d8aa1-8135-4f8a-9132-54e77a4c4b2b')) THEN
    delete from stormac where primarykey = '561d8aa1-8135-4f8a-9132-54e77a4c4b2b';
	insert into stormac (primarykey, typeaccess, filter_m0, permition_m0, createtime, creator, edittime, editor)
    values ('9f2b1af3-a489-4cbc-9dce-80eaa04a9695', 'Read', null, 'e5c036e9-5927-491b-9f66-9f885cd395d9', now() at time zone 'utc', null, null, null),
    ('7df98203-8019-49f5-8fb3-9300846f05e9', 'Update', null, 'e5c036e9-5927-491b-9f66-9f885cd395d9', now() at time zone 'utc', null, null, null),
    ('035c7990-0eab-47bb-8a9d-bdf281cefad4', 'Delete', null, 'e5c036e9-5927-491b-9f66-9f885cd395d9', now() at time zone 'utc', null, null, null),
    ('018c0ec9-b4aa-45a4-a47f-a6123e0e674a', 'Execute', null, 'e5c036e9-5927-491b-9f66-9f885cd395d9', now() at time zone 'utc', null, null, null)
	ON CONFLICT (primarykey) DO NOTHING;
	END IF;

	INSERT INTO stormag (primarykey,  Name, IsUser, IsGroup, IsRole, Enabled, CreateTime, Creator, town)
	VALUES ('0e68a31e-e378-4817-9e8d-15c468f175cd', 'userAdmin', false, false, true, true, now() at time zone 'utc', 'admin', '521fb267-b7c1-4169-b435-ae126b7938b8')
	ON CONFLICT (primarykey) DO NOTHING;

	insert into stormp (primarykey, subject_m0, agent_m0, createtime, creator, edittime, editor)
    values ('5117d9d1-cc7c-42d7-b0e5-4ef2c8c3f195', '019274ed-492b-716e-91a5-0c28bbf872d4', '0e68a31e-e378-4817-9e8d-15c468f175cd', now() at time zone 'utc', null, null, null)
	ON CONFLICT (primarykey) DO NOTHING;

	insert into stormac (primarykey, typeaccess, filter_m0, permition_m0, createtime, creator, edittime, editor)
	values ('2d2f82ef-59a8-4bba-9104-8b92849bf6e3', 'Insert', null, '5117d9d1-cc7c-42d7-b0e5-4ef2c8c3f195', now() at time zone 'utc', null, null, null)
	ON CONFLICT (primarykey) DO NOTHING;

END;
$$ LANGUAGE PLPGSQL;

--rollback DO $$ BEGIN DELETE FROM stormac WHERE primarykey IN ('295a0754-708e-4497-a7d8-1c9f802de1c6','1f618b79-ce76-40f5-8574-d505c2d1be8f','cf7e70ed-f133-4eae-bf1d-ee8ab36998ab','c4826477-b167-47b7-94d5-126eb0eb3cea','9f2b1af3-a489-4cbc-9dce-80eaa04a9695','7df98203-8019-49f5-8fb3-9300846f05e9','035c7990-0eab-47bb-8a9d-bdf281cefad4','018c0ec9-b4aa-45a4-a47f-a6123e0e674a'); INSERT INTO stormac (primarykey,typeaccess,filter_m0,permition_m0,createtime,creator,edittime,editor) VALUES ('019274fa-b78c-7e76-a8a3-9ff071c80d34','Full',null,'019274fa-b78c-769c-bc2d-3ab94aefd6dc',now() at time zone 'utc',null,null,null),('561d8aa1-8135-4f8a-9132-54e77a4c4b2b','Full',null,'e5c036e9-5927-491b-9f66-9f885cd395d9',now() at time zone 'utc',null,null,null) ON CONFLICT (primarykey) DO NOTHING; delete from stormac where primarykey in ('2d2f82ef-59a8-4bba-9104-8b92849bf6e3'); delete from stormp where primarykey in ('5117d9d1-cc7c-42d7-b0e5-4ef2c8c3f195'); delete from stormag where primarykey in ('0e68a31e-e378-4817-9e8d-15c468f175cd'); END; $$ LANGUAGE PLPGSQL;