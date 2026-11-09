--liquibase formatted sql
--changeset vborovin:permissionsForAwpRedinessPassport

select ChangeOrCreatePermissionFor('Full', 'admin', 'Tmobis.Business.Process.Documents.AwpReadinessPassport');
select ChangeOrCreatePermissionFor('Full', 'Пользователь', 'Tmobis.Business.Process.Documents.AwpReadinessPassport');
select ChangeOrCreatePermissionFor('Full', 'admin', 'Tmobis.Business.Process.Documents.AwpReadinessPassportObject');
select ChangeOrCreatePermissionFor('Full', 'Пользователь', 'Tmobis.Business.Process.Documents.AwpReadinessPassportObject');