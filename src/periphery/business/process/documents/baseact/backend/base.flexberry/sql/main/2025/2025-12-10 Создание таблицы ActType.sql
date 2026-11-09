--liquibase formatted sql
--changeset ValiullinNR:CreateActType

CREATE TABLE IF NOT EXISTS ActType
(
    primarykey uuid NOT NULL,
    name VARCHAR(200) NOT NULL,
	code VARCHAR(10) NULL,
    Acronym VARCHAR(10) NOT NULL,
	ShortName VARCHAR(100),
	EditFormName VARCHAR(10),
	actuality boolean NOT NULL,
    CreateTime TIMESTAMP(3) NULL,
	Creator VARCHAR(255) NULL,
	Editor VARCHAR(255) NULL,
	EditTime TIMESTAMP(3) NULL,
    PRIMARY KEY (primaryKey));
	
INSERT INTO ActType 
(primarykey, name, Acronym, ShortName, EditFormName, actuality, CreateTime, Creator, Editor, EditTime) 
VALUES 
('2653b090-817a-4f29-9336-b95159e8878e', 'Оценочный лист', 'ОЛ', null, null, true, NOW(), 'admin', null, null), 
('9d3541bb-d476-47fd-95de-83b198bcdf97', 'Акт оценки обеспечения готовности к отопительному периоду', 'ООГ', null, null, true, NOW(), 'admin', null, null), 
('3979873a-eb17-4bf4-bd29-62ded53f8b5b', 'Паспорт обеспечения готовности к отопительному периоду', 'ПОГ', null, null, true, NOW(), 'admin', null, null)

--rollback DROP TABLE IF EXISTS ActType;


