--liquibase formatted sql
--changeset vborovin:permsAwpAssessingProgram

select ChangeOrCreatePermissionFor('Full', 'admin', 'Tmobis.Business.Process.Documents.AwpAssessingProgram');
select ChangeOrCreatePermissionFor('Full', 'Пользователь', 'Tmobis.Business.Process.Documents.AwpAssessingProgram');