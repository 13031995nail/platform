--liquibase formatted sql
--changeset lprotasov:permissionsEvaluationSheetRequirement

select ChangeOrCreatePermissionFor('Full', 'admin', 'Tmobis.Business.Process.Documents.EvaluationSheetRequirement');
select ChangeOrCreatePermissionFor('Full', 'Пользователь', 'Tmobis.Business.Process.Documents.EvaluationSheetRequirement');
