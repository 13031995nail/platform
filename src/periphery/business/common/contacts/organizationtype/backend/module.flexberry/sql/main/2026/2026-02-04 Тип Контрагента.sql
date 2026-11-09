--liquibase formatted sql
--changeset ypolezhaev:2026-02-04-organization-type
CREATE TABLE IF NOT EXISTS OrganizationType (
primaryKey UUID NOT NULL,
CreateTime TIMESTAMP(3) NULL,
Creator VARCHAR(255) NULL,
Editor VARCHAR(255) NULL,
EditTime TIMESTAMP(3) NULL,
Code VARCHAR(10) NULL,
Name VARCHAR(256) NOT NULL,
Actuality boolean NOT NULL,
PRIMARY KEY (primaryKey)
);

INSERT INTO OrganizationType 
(primarykey, name, actuality, CreateTime, Creator, Editor, EditTime) 
VALUES 
('7909c321-f77c-4bdf-9d4d-e618fac50c81', 'Муниципальное образование', true, NOW(), 'admin', null, null)
ON CONFLICT DO NOTHING;

select ChangeOrCreatePermissionFor('Full', 'admin', 'Tmobis.Business.Common.Contacts.OrganizationType');
select ChangeOrCreatePermissionFor('Read', 'Пользователь', 'Tmobis.Business.Common.Contacts.OrganizationType');