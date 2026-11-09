--liquibase formatted sql
--changeset nvaliullin:addOrganizationHierarchyName

ALTER TABLE organization ADD COLUMN IF NOT EXISTS HierarchyName VARCHAR(150) NULL;

UPDATE organization as org1
SET HierarchyName = org2.name
FROM organization as org2
WHERE org1.hierarchy = org2.primarykey;
