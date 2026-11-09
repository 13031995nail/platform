--liquibase formatted sql
--changeset lprotasov:addEvaluationSheetFields

ALTER TABLE EvaluationSheet ADD COLUMN IF NOT EXISTS workObject UUID NULL;
ALTER TABLE EvaluationSheet ADD COLUMN IF NOT EXISTS awp UUID NULL;
ALTER TABLE EvaluationSheet ADD COLUMN IF NOT EXISTS representative VARCHAR(250) NULL;

CREATE INDEX IF NOT EXISTS EvaluationSheet_IWorkObject ON EvaluationSheet (workObject);
ALTER TABLE EvaluationSheet ADD CONSTRAINT EvaluationSheet_FWorkObject_0 FOREIGN KEY (workObject) REFERENCES WorkObject;

CREATE INDEX IF NOT EXISTS EvaluationSheet_IAwp ON EvaluationSheet (awp);
ALTER TABLE EvaluationSheet ADD CONSTRAINT EvaluationSheet_FAwp_0 FOREIGN KEY (awp) REFERENCES Awp;