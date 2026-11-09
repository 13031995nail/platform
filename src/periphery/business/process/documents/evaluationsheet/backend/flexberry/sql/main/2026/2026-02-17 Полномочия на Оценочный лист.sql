--liquibase formatted sql
--changeset lprotasov:permissionsEvaluationSheet

select ChangeOrCreatePermissionFor('Full', 'admin', 'Tmobis.Business.Process.Documents.EvaluationSheet');
select ChangeOrCreatePermissionFor('Full', 'Пользователь', 'Tmobis.Business.Process.Documents.EvaluationSheet');
