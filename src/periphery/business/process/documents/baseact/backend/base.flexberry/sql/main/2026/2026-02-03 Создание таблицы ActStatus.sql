--liquibase formatted sql
--changeset vborovin:CreateActStatus

CREATE TABLE IF NOT EXISTS ActStatus
(
    primarykey uuid NOT NULL,
    name VARCHAR(200) NOT NULL,
	code VARCHAR(10) NULL,
	actuality boolean NOT NULL,
    createTime TIMESTAMP(3) NULL,
	creator VARCHAR(255) NULL,
	editor VARCHAR(255) NULL,
	editTime TIMESTAMP(3) NULL,
    PRIMARY KEY (primaryKey));
	
INSERT INTO ActStatus
(primarykey, name, actuality, createTime, creator, editor, editTime) 
VALUES 
('119719b2-e3bd-4f00-a051-2426220d3050', 'Создан', true, NOW(), 'admin', null, null),
('119719b2-e3bd-4f00-a051-2426220d3051', 'На редактировании', true, NOW(), 'admin', null, null),
('119719b2-e3bd-4f00-a051-2426220d3052', 'На подписании', true, NOW(), 'admin', null, null),
('119719b2-e3bd-4f00-a051-2426220d3053', 'Подписан', true, NOW(), 'admin', null, null),
('119719b2-e3bd-4f00-a051-2426220d3054', 'Утвержден', true, NOW(), 'admin', null, null);

--rollback DROP TABLE IF EXISTS ActStatus;