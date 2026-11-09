--liquibase formatted sql
--changeset vborovin:actTypeProgram
	
INSERT INTO ActType 
(primarykey, name, Acronym, ShortName, EditFormName, actuality, CreateTime, Creator, Editor, EditTime) 
VALUES 
('32c1d050-7812-4e8b-9daa-257c09ecaf59', 'Программа проведения оценки обеспечения готовности к ОП', 'ПП', null, null, true, NOW(), 'admin', null, null)