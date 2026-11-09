--liquibase formatted sql
--changeset kmenshchikova:createRedinessAct

CREATE TABLE IF NOT EXISTS RedinessAct
(
	-- BaseActBase
    primarykey uuid NOT NULL,
    regDate TIMESTAMP(3) NOT NULL,
    actStatus uuid NOT NULL,
    actType uuid NOT NULL,
    actNumber VARCHAR(50) NOT NULL,
    changeStatusDate TIMESTAMP(3) NULL,
    actNumberIndex int NULL,
    assignee uuid NOT NULL,
	
	-- BaseAct
    organizationName VARCHAR(500) NULL,
	organizationPhone VARCHAR(250) NULL,
	organizationAddress VARCHAR(250) NULL,
    organization uuid NOT NULL,

	-- RedinessAct
	awp uuid NOT NULL,
	awpAssessingProgram uuid NOT NULL,
	partnerReadiness VARCHAR(20) NULL,
	
	-- Audit
    createTime TIMESTAMP(3) NULL,
	creator VARCHAR(255) NULL,
	editor VARCHAR(255) NULL,
	editTime TIMESTAMP(3) NULL,
    PRIMARY KEY (primaryKey),
	FOREIGN KEY (actStatus) REFERENCES ActStatus,
	FOREIGN KEY (actType) REFERENCES ActType,
	FOREIGN KEY (organization) REFERENCES Organization,
	FOREIGN KEY (awp) REFERENCES Awp,
	FOREIGN KEY (awpAssessingProgram) REFERENCES AwpAssessingProgram
);

--rollback DROP TABLE IF EXISTS RedinessAct;


