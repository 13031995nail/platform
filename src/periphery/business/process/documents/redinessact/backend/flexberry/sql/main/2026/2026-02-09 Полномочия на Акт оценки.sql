--liquibase formatted sql
--changeset kmenshchikova:permissionsForRedinessAct

select ChangeOrCreatePermissionFor('Full', 'admin', 'Tmobis.Business.Process.Documents.RedinessAct');
select ChangeOrCreatePermissionFor('Full', 'Пользователь', 'Tmobis.Business.Process.Documents.RedinessAct');
select ChangeOrCreatePermissionFor('Full', 'admin', 'Tmobis.Business.Process.Documents.RedinessActObject');
select ChangeOrCreatePermissionFor('Full', 'Пользователь', 'Tmobis.Business.Process.Documents.RedinessActObject');