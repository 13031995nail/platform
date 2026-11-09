--liquibase formatted sql
--changeset kmenshchikova:createRedinessActObject

CREATE TABLE IF NOT EXISTS RedinessActObject
(
    primarykey uuid NOT NULL,
    workObject uuid NOT NULL,
    act uuid NOT NULL,
	
	-- Audit
    createTime TIMESTAMP(3) NULL,
	creator VARCHAR(255) NULL,
	editor VARCHAR(255) NULL,
	editTime TIMESTAMP(3) NULL,

    PRIMARY KEY (primaryKey),
	FOREIGN KEY (workObject) REFERENCES WorkObject,
	FOREIGN KEY (act) REFERENCES RedinessAct
);

--rollback DROP TABLE IF EXISTS RedinessActObject;


