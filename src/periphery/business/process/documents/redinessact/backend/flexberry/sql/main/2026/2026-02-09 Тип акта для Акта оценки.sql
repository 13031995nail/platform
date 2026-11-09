--liquibase formatted sql
--changeset kmenshchikova:actTypeRedinessAct
	
INSERT INTO ActType 
(primarykey, name, Acronym, ShortName, EditFormName, actuality, CreateTime, Creator, Editor, EditTime) 
VALUES 
('22e064cf-1d25-4fbe-b192-d969fb15725b', 'Акт оценки обеспечения готовности к отопительному периоду', 'ООГ', null, null, true, NOW(), 'admin', null, null)

--rollback DELETE from ActType where primarykey='22e064cf-1d25-4fbe-b192-d969fb15725b'
