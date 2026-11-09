--liquibase formatted sql
--changeset kmenshchikova:createAwpAssessingProgramSign

CREATE TABLE IF NOT EXISTS AwpAssessingProgramSign
(
    primarykey uuid NOT NULL,
    signature varchar(50) NULL,
    signdate timestamp NULL,
    signrole uuid NULL,
    signtype uuid NULL,
    act uuid NULL,
    employee uuid NULL,
    
    fio text NULL,
	address text NULL,
	passport text NULL,
	phone text NULL,
	email varchar(150) NULL,
	positionname text NULL,
	partnername text NULL,
	
	-- Audit
    createTime TIMESTAMP(3) NULL,
	creator VARCHAR(255) NULL,
	editor VARCHAR(255) NULL,
	editTime TIMESTAMP(3) NULL,

    PRIMARY KEY (primaryKey),
	FOREIGN KEY (signrole) REFERENCES SignRole,
	FOREIGN KEY (signtype) REFERENCES SignType,
	FOREIGN KEY (act) REFERENCES AwpAssessingProgram,
  FOREIGN KEY (employee) REFERENCES Employee
);

select ChangeOrCreatePermissionFor('Full', 'admin', 'Tmobis.Business.Process.Documents.AwpAssessingProgramSign');
select ChangeOrCreatePermissionFor('Full', 'Пользователь', 'Tmobis.Business.Process.Documents.AwpAssessingProgramSign');

--rollback DROP TABLE IF EXISTS AwpAssessingProgramActSign;



