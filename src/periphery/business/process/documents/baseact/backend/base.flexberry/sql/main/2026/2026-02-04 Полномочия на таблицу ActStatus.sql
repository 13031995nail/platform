--liquibase formatted sql
--changeset vborovin:ActStatusPerms

select ChangeOrCreatePermissionFor('Full', 'admin', 'Tmobis.Business.Process.Documents.Flexberry.ActStatus');
select ChangeOrCreatePermissionFor('Read', 'Пользователь', 'Tmobis.Business.Process.Documents.Flexberry.ActStatus');