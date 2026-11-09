--liquibase formatted sql
--changeset vborovin:stormagperms

select ChangeOrCreatePermissionFor('Full', 'admin', 'ICSSoft.STORMNET.Security.Agent');
select ChangeOrCreatePermissionFor('Read', 'Пользователь', 'ICSSoft.STORMNET.Security.Agent');