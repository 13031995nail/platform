--liquibase formatted sql
--changeset ValiullinNR:CreateEmployeeSignature

CREATE TABLE IF NOT EXISTS EmployeeSignature
(
    primarykey uuid NOT NULL,
    CreateTime TIMESTAMP(3) NULL,
	Creator VARCHAR(255) NULL,
	Editor VARCHAR(255) NULL,
	EditTime TIMESTAMP(3) NULL,
	Signature VARCHAR(50) NULL,
	Employee UUID NOT NULL,
    PRIMARY KEY (primaryKey));
	
ALTER TABLE EmployeeSignature DROP CONSTRAINT IF EXISTS EmployeeSignature_FEmployee_0;

drop index IF EXISTS EmployeeSignature_IEmployee;

ALTER TABLE  EmployeeSignature ADD CONSTRAINT EmployeeSignature_FEmployee_0 FOREIGN KEY (Employee) REFERENCES Employee; 
CREATE INDEX EmployeeSignature_IEmployee on EmployeeSignature (Employee);

--rollback ALTER TABLE EmployeeSignature DROP CONSTRAINT IF EXISTS EmployeeSignature_FEmployee_0;drop index IF EXISTS EmployeeSignature_IEmployee;DROP TABLE IF EXISTS EmployeeSignature;


