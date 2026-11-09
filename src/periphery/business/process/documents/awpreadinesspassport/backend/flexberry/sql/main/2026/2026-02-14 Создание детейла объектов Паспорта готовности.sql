--liquibase formatted sql
--changeset vborovin:createReadinessPassportObject

CREATE TABLE IF NOT EXISTS AwpReadinessPassportObject
(
    primarykey uuid NOT NULL,
    workObject uuid NOT NULL,
    number int NOT NULL,
    act uuid NOT NULL,
	
	-- Audit
    createTime TIMESTAMP(3) NULL,
	creator VARCHAR(255) NULL,
	editor VARCHAR(255) NULL,
	editTime TIMESTAMP(3) NULL,

    PRIMARY KEY (primaryKey),
	FOREIGN KEY (workObject) REFERENCES WorkObject,
	FOREIGN KEY (act) REFERENCES AwpReadinessPassport
);

--rollback DROP TABLE IF EXISTS AwpReadinessPassportObject;


