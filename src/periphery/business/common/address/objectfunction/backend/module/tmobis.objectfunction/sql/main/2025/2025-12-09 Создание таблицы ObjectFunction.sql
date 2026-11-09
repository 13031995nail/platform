--liquibase formatted sql
--changeset ValiullinNR:CreateObjectFunction

CREATE TABLE IF NOT EXISTS ObjectFunction
(
    primarykey uuid NOT NULL,
	code VARCHAR(10) NULL,
    name VARCHAR(200) NOT NULL,
	actuality boolean NOT NULL,
    CreateTime TIMESTAMP(3) NULL,
	Creator VARCHAR(255) NULL,
	Editor VARCHAR(255) NULL,
	EditTime TIMESTAMP(3) NULL,
    PRIMARY KEY (primaryKey));

--rollback DROP TABLE IF EXISTS ObjectFunction;


