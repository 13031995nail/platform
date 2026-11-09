--liquibase formatted sql
--changeset lprotasov:permissionsTSOEvaluationSheetRequirement

select ChangeOrCreatePermissionFor('Full', 'ТСО', 'Tmobis.Business.Process.Documents.EvaluationSheetRequirement');
select ChangeOrCreatePermissionFor('Full', 'ТСО', 'Tmobis.Business.Process.Documents.EvaluationSheet');
