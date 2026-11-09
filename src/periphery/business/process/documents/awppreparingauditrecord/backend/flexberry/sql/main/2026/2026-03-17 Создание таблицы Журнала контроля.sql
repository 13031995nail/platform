--liquibase formatted sql
--changeset kmenshchikova:СreateAwpPreparingAuditRecord

CREATE TABLE IF NOT EXISTS AwpPreparingAuditRecord
(
    primarykey uuid NOT NULL,
    awpReadinessPassport uuid NULL,
    passportDate TIMESTAMP(3) NULL,
    evaluationSheet uuid NULL,
    evaluationSheetDate TIMESTAMP(3) NULL,
    readinessAct uuid NULL,
    readinessActDate TIMESTAMP(3) NULL,
    indexReadiness DECIMAL(18,2) NULL,
    indexPartnerReadiness DECIMAL(18,2) NULL,
    partnerReadiness VARCHAR(20) NULL,
    awp uuid NOT NULL,
    workObject uuid NOT NULL,
    
	-- Audit
    createTime TIMESTAMP(3) NULL,
	creator VARCHAR(255) NULL,
	editor VARCHAR(255) NULL,
	editTime TIMESTAMP(3) NULL,
	
    PRIMARY KEY (primaryKey),
	FOREIGN KEY (evaluationSheet) REFERENCES EvaluationSheet,
	FOREIGN KEY (readinessAct) REFERENCES RedinessAct,
	FOREIGN KEY (awpReadinessPassport) REFERENCES AwpReadinessPassport,
	FOREIGN KEY (awp) REFERENCES Awp,
	FOREIGN KEY (workObject) REFERENCES WorkObject
);

--rollback DROP TABLE IF EXISTS AwpPreparingAuditRecord;


