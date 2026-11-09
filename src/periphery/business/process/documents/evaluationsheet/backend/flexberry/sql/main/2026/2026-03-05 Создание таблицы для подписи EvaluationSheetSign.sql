--liquibase formatted sql
--changeset kmenshchikova:createEvaluationSheetSign

CREATE TABLE IF NOT EXISTS EvaluationSheetSign
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
	FOREIGN KEY (act) REFERENCES EvaluationSheet,
  FOREIGN KEY (employee) REFERENCES Employee
);

select ChangeOrCreatePermissionFor('Full', 'admin', 'Tmobis.Business.Process.Documents.EvaluationSheetSign');
select ChangeOrCreatePermissionFor('Full', 'Пользователь', 'Tmobis.Business.Process.Documents.EvaluationSheetSign');

--rollback DROP TABLE IF EXISTS EvaluationSheetSign;



