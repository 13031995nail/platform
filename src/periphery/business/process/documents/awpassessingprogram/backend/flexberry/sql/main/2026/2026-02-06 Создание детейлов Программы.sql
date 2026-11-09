--liquibase formatted sql
--changeset vborovin:createAwpAssessingProgramDetails

CREATE TABLE IF NOT EXISTS AwpAssessingProgramEvaluationSchedule
(
    primarykey uuid NOT NULL,
    startDate TIMESTAMP(3) NOT NULL,
	finishDate TIMESTAMP(3) NOT NULL,
    objectFunction uuid NOT NULL,
    program uuid NOT NULL,
	
	-- Audit
    createTime TIMESTAMP(3) NULL,
	creator VARCHAR(255) NULL,
	editor VARCHAR(255) NULL,
	editTime TIMESTAMP(3) NULL,

    PRIMARY KEY (primaryKey),
	FOREIGN KEY (objectFunction) REFERENCES ObjectFunction,
	FOREIGN KEY (program) REFERENCES AwpAssessingProgram
);

CREATE TABLE IF NOT EXISTS AwpAssessingProgramObject
(
    primarykey uuid NOT NULL,
    workObject uuid NOT NULL,
    program uuid NOT NULL,
	
	-- Audit
    createTime TIMESTAMP(3) NULL,
	creator VARCHAR(255) NULL,
	editor VARCHAR(255) NULL,
	editTime TIMESTAMP(3) NULL,

    PRIMARY KEY (primaryKey),
	FOREIGN KEY (workObject) REFERENCES WorkObject,
	FOREIGN KEY (program) REFERENCES AwpAssessingProgram
);

--rollback DROP TABLE IF EXISTS AwpAssessingProgramEvaluationSchedule; DROP TABLE IF EXISTS AwpAssessingProgramObject;


