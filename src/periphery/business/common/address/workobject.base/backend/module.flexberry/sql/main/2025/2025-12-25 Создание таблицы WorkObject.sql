--liquibase formatted sql
--changeset ValiullinNR:CreateWorkObject

CREATE TABLE IF NOT EXISTS WorkObject
(
    primarykey uuid NOT NULL,
    name VARCHAR(500) NOT NULL,
	actuality boolean NOT NULL,
    CreateTime TIMESTAMP(3) NULL,
	Creator VARCHAR(255) NULL,
	Editor VARCHAR(255) NULL,
	EditTime TIMESTAMP(3) NULL,
	AddressString VARCHAR(350) NOT NULL,
	ObjectFunction UUID NOT NULL,
	Organization UUID NOT NULL,
	Hierarchy UUID NULL,
    PRIMARY KEY (primaryKey));
	
ALTER TABLE WorkObject DROP CONSTRAINT IF EXISTS WorkObject_FObjectFunction_0;

ALTER TABLE WorkObject DROP CONSTRAINT IF EXISTS WorkObject_FOrganization_0;

ALTER TABLE WorkObject DROP CONSTRAINT IF EXISTS WorkObject_FHierarchy_0;

drop index IF EXISTS WorkObject_IObjectFunction;

drop index IF EXISTS WorkObject_IOrganization;

drop index IF EXISTS WorkObject_IHierarchy;

ALTER TABLE WorkObject ADD CONSTRAINT WorkObject_FObjectFunction_0 FOREIGN KEY (ObjectFunction) REFERENCES ObjectFunction; 
CREATE INDEX WorkObject_IObjectFunction on WorkObject (ObjectFunction);

ALTER TABLE WorkObject ADD CONSTRAINT WorkObject_FOrganization_0 FOREIGN KEY (Organization) REFERENCES Organization; 
CREATE INDEX WorkObject_IOrganization on WorkObject (Organization);

ALTER TABLE WorkObject ADD CONSTRAINT WorkObject_FHierarchy_0 FOREIGN KEY (Hierarchy) REFERENCES WorkObject; 
CREATE INDEX WorkObject_IHierarchy on WorkObject (Hierarchy);

--rollback ALTER TABLE WorkObject DROP CONSTRAINT IF EXISTS WorkObject_FHierarchy_0;ALTER TABLE WorkObject DROP CONSTRAINT IF EXISTS WorkObject_FObjectFunction_0;ALTER TABLE WorkObject DROP CONSTRAINT IF EXISTS WorkObject_FOrganization_0;drop index IF EXISTS WorkObject_IObjectFunction;drop index IF EXISTS WorkObject_IOrganization;drop index IF EXISTS WorkObject_IHierarchy;DROP TABLE IF EXISTS WorkObject;


