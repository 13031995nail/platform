--liquibase formatted sql
--changeset kmenshchikova:PermissionsForAwpPreparingAuditRecord

select ChangeOrCreatePermissionFor('Full', 'admin', 'Tmobis.Business.Process.Documents.AwpPreparingAuditRecord');
select ChangeOrCreatePermissionFor('Full', 'Пользователь', 'Tmobis.Business.Process.Documents.AwpPreparingAuditRecord');
