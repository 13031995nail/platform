--liquibase formatted sql
--changeset vborovin:permsAwpAssessingProgramDetails

select ChangeOrCreatePermissionFor('Full', 'admin', 'Tmobis.Business.Process.Documents.AwpAssessingProgramObject');
select ChangeOrCreatePermissionFor('Full', 'Пользователь', 'Tmobis.Business.Process.Documents.AwpAssessingProgramObject');
select ChangeOrCreatePermissionFor('Full', 'admin', 'Tmobis.Business.Process.Documents.AwpAssessingProgramEvaluationSchedule');
select ChangeOrCreatePermissionFor('Full', 'Пользователь', 'Tmobis.Business.Process.Documents.AwpAssessingProgramEvaluationSchedule');