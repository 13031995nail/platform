--liquibase formatted sql
--changeset vborovin:corePermsRename

select ChangeOrCreatePermissionFor('Full', 'admin', 'ICSSoft.STORMNET.Security.Subject');
select ChangeOrCreatePermissionFor('Read', 'Пользователь', 'ICSSoft.STORMNET.Security.Subject');