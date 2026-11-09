--liquibase formatted sql
--changeset ValiullinNR:CreateAdmissionCommittee

CREATE TABLE IF NOT EXISTS AdmissionCommittee
(
    primarykey uuid NOT NULL,
    CreateTime TIMESTAMP(3) NULL,
	Creator VARCHAR(255) NULL,
	Editor VARCHAR(255) NULL,
	EditTime TIMESTAMP(3) NULL,
	Status VARCHAR(20) NULL,
	ApprovedDate TIMESTAMP(3) NOT NULL,
	Leader UUID NULL,
	Awp UUID NOT NULL,
	Organization UUID NOT NULL,
    PRIMARY KEY (primaryKey));
	
CREATE TABLE IF NOT EXISTS AdmissionCommitteeEmployee
(
    primarykey uuid NOT NULL,
    CreateTime TIMESTAMP(3) NULL,
	Creator VARCHAR(255) NULL,
	Editor VARCHAR(255) NULL,
	EditTime TIMESTAMP(3) NULL,
	Employee UUID NOT NULL,
	AdmissionCommittee UUID NOT NULL,
    PRIMARY KEY (primaryKey));
	
ALTER TABLE AdmissionCommittee DROP CONSTRAINT IF EXISTS AdmissionCommittee_FLeader_0;

ALTER TABLE AdmissionCommittee DROP CONSTRAINT IF EXISTS AdmissionCommittee_FAwp_0;

ALTER TABLE AdmissionCommittee DROP CONSTRAINT IF EXISTS AdmissionCommittee_FOrganization_0;

ALTER TABLE AdmissionCommitteeEmployee DROP CONSTRAINT IF EXISTS AdmissionCommitteeEmployee_FEmployee_0;

ALTER TABLE AdmissionCommitteeEmployee DROP CONSTRAINT IF EXISTS AdmissionCommitteeEmployee_FAdmissionCommittee_0;

drop index IF EXISTS AdmissionCommittee_ILeader;

drop index IF EXISTS AdmissionCommittee_IAwp;

drop index IF EXISTS AdmissionCommittee_IOrganization;

drop index IF EXISTS AdmissionCommitteeEmployee_IEmployee;

drop index IF EXISTS AdmissionCommitteeEmployee_IAdmissionCommittee;

ALTER TABLE AdmissionCommittee ADD CONSTRAINT AdmissionCommittee_FLeader_0 FOREIGN KEY (Leader) REFERENCES AdmissionCommitteeEmployee; 
CREATE INDEX AdmissionCommittee_ILeader on AdmissionCommittee (Leader);

ALTER TABLE AdmissionCommittee ADD CONSTRAINT AdmissionCommittee_FAwp_0 FOREIGN KEY (Awp) REFERENCES Awp; 
CREATE INDEX AdmissionCommittee_IAwp on AdmissionCommittee (Awp);

ALTER TABLE AdmissionCommittee ADD CONSTRAINT AdmissionCommittee_FOrganization_0 FOREIGN KEY (Organization) REFERENCES Organization; 
CREATE INDEX AdmissionCommittee_IOrganization on AdmissionCommittee (Organization);

ALTER TABLE AdmissionCommitteeEmployee ADD CONSTRAINT AdmissionCommitteeEmployee_FEmployee_0 FOREIGN KEY (Employee) REFERENCES Employee; 
CREATE INDEX AdmissionCommitteeEmployee_IEmployee on AdmissionCommitteeEmployee (Employee);

ALTER TABLE AdmissionCommitteeEmployee ADD CONSTRAINT AdmissionCommitteeEmployee_FAdmissionCommittee_0 FOREIGN KEY (AdmissionCommittee) REFERENCES AdmissionCommittee; 
CREATE INDEX AdmissionCommitteeEmployee_IAdmissionCommittee on AdmissionCommitteeEmployee (AdmissionCommittee);

--rollback ALTER TABLE AdmissionCommittee DROP CONSTRAINT IF EXISTS AdmissionCommittee_FOrganization_0; ALTER TABLE AdmissionCommittee DROP CONSTRAINT IF EXISTS AdmissionCommittee_FAwp_0; ALTER TABLE AdmissionCommittee DROP CONSTRAINT IF EXISTS AdmissionCommittee_FLeader_0;ALTER TABLE AdmissionCommitteeEmployee DROP CONSTRAINT IF EXISTS AdmissionCommitteeEmployee_FEmployee_0;ALTER TABLE AdmissionCommitteeEmployee DROP CONSTRAINT IF EXISTS AdmissionCommitteeEmployee_FAdmissionCommittee_0;DROP TABLE IF EXISTS AdmissionCommittee;DROP TABLE IF EXISTS AdmissionCommitteeEmployee;


