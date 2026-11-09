--liquibase formatted sql
--changeset lprotasov:createTSORole

INSERT INTO STORMAG(primaryKey,  Name, IsUser, IsGroup, IsRole, Enabled, CreateTime, Creator, town)
VALUES ('a1b2c3d4-e5f6-7890-abcd-ef1234567890', 'ТСО', false, false, true, true, NOW(), 'admin', '521fb267-b7c1-4169-b435-ae126b7938b8')
ON CONFLICT (primaryKey) DO NOTHING;

--rollback delete from stormag where primarykey in ('a1b2c3d4-e5f6-7890-abcd-ef1234567890')
--comment createTSORole
