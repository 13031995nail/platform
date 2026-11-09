--liquibase formatted sql
--changeset lprotasov:createEvaluationSheetRequirement

CREATE TABLE IF NOT EXISTS EvaluationSheetRequirement
(
    primarykey uuid NOT NULL,
    evaluationSheet uuid NOT NULL,
    verificationRequirement uuid NOT NULL,
    supportingDocName VARCHAR(2000) NULL,
    supportingDocFile VARCHAR(1024) NULL,
    calculatedReq DECIMAL(18,2) NULL,
    isCompletedReq BOOLEAN NOT NULL DEFAULT false,
    problemComment VARCHAR(4000) NULL,
    deadline TIMESTAMP(3) NULL,
    createTime TIMESTAMP(3) NULL,
    creator VARCHAR(255) NULL,
    editor VARCHAR(255) NULL,
    editTime TIMESTAMP(3) NULL,
    PRIMARY KEY (primaryKey),
    FOREIGN KEY (evaluationSheet) REFERENCES EvaluationSheet,
    FOREIGN KEY (verificationRequirement) REFERENCES verificationrequierement
);

--rollback DROP TABLE IF EXISTS EvaluationSheetRequirement;
