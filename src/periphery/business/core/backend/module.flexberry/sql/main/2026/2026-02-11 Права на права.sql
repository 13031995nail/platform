--liquibase formatted sql
--changeset vborovin:permsperms

select ChangeOrCreatePermissionFor('Full', 'admin', 'ICSSoft.STORMNET.Security.LinkRole');
select ChangeOrCreatePermissionFor('Read', 'Пользователь', 'ICSSoft.STORMNET.Security.LinkRole');
select ChangeOrCreatePermissionFor('Full', 'admin', 'ICSSoft.STORMNET.Security.Filter');
select ChangeOrCreatePermissionFor('Read', 'Пользователь', 'ICSSoft.STORMNET.Security.Filter');
select ChangeOrCreatePermissionFor('Full', 'admin', 'ICSSoft.STORMNET.Security.Permition');
select ChangeOrCreatePermissionFor('Read', 'Пользователь', 'ICSSoft.STORMNET.Security.Permition');
select ChangeOrCreatePermissionFor('Full', 'admin', 'ICSSoft.STORMNET.Security.Access');
select ChangeOrCreatePermissionFor('Read', 'Пользователь', 'ICSSoft.STORMNET.Security.Access');