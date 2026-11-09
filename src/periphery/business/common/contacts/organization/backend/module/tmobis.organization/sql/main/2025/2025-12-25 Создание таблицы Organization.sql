--liquibase formatted sql
--changeset ValiullinNR:CreateOrganization

CREATE TABLE IF NOT EXISTS Organization
(
    primarykey uuid NOT NULL,
    name VARCHAR(200) NOT NULL,
	actuality boolean NOT NULL,
    CreateTime TIMESTAMP(3) NULL,
	Creator VARCHAR(255) NULL,
	Editor VARCHAR(255) NULL,
	EditTime TIMESTAMP(3) NULL,
	Address VARCHAR(350) NULL,
	LegalAddress VARCHAR(350) NULL,
	Phone VARCHAR(50) NULL,
	Email VARCHAR(250) NULL,
	Acronym VARCHAR(10) NULL,
	Town UUID NOT NULL,
	Hierarchy UUID NULL,
	OrganizationType UUID NOT NULL,
    PRIMARY KEY (primaryKey));
	
ALTER TABLE Organization DROP CONSTRAINT IF EXISTS Organization_FTown_0;

ALTER TABLE Organization DROP CONSTRAINT IF EXISTS Organization_FHierarchy_0;

ALTER TABLE Organization DROP CONSTRAINT IF EXISTS Organization_FOrganizationType_0;

drop index IF EXISTS Organization_ITown;

drop index IF EXISTS Organization_IHierarchy;

drop index IF EXISTS Organization_IOrganizationType;

ALTER TABLE Organization ADD CONSTRAINT Organization_FTown_0 FOREIGN KEY (Town) REFERENCES Town; 
CREATE INDEX Organization_ITown on Organization (Town);

ALTER TABLE Organization ADD CONSTRAINT Organization_FHierarchy_0 FOREIGN KEY (Hierarchy) REFERENCES Organization; 
CREATE INDEX Organization_IHierarchy on Organization (Hierarchy);

ALTER TABLE Organization ADD CONSTRAINT Organization_FOrganizationType_0 FOREIGN KEY (OrganizationType) REFERENCES OrganizationType; 
CREATE INDEX Organization_IOrganizationType on Organization (OrganizationType); 

--rollback ALTER TABLE Organization DROP CONSTRAINT IF EXISTS Organization_FTown_0;ALTER TABLE Organization DROP CONSTRAINT IF EXISTS Organization_FHierarchy_0;ALTER TABLE Organization DROP CONSTRAINT IF EXISTS Organization_FOrganizationType_0;drop index IF EXISTS Organization_ITown;drop index IF EXISTS Organization_IHierarchy;drop index IF EXISTS Organization_IOrganizationType;DROP TABLE IF EXISTS Organization;


