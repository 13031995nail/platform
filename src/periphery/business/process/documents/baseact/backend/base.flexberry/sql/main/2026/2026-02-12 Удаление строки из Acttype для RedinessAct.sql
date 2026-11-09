 --liquibase formatted sql
--changeset kmenshchikova:dubleActTypeRedinessAct
	   delete from acttype where primarykey='9d3541bb-d476-47fd-95de-83b198bcdf97' and exists (SELECT 1
	                FROM acttype
	                WHERE primarykey='9d3541bb-d476-47fd-95de-83b198bcdf97' and acronym='ООГ');

 --rollback INSERT INTO ActType (primarykey, name, Acronym, ShortName, EditFormName, actuality, CreateTime, Creator, Editor, EditTime) VALUES ('9d3541bb-d476-47fd-95de-83b198bcdf97', 'Акт оценки обеспечения готовности к отопительному периоду', 'ООГ', null, null, true, NOW(), 'admin', null, null);
