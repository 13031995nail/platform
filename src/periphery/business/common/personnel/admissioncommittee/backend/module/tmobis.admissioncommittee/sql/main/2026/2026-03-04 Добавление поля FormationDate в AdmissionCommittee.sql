--liquibase formatted sql
--changeset nvaliullin:addFormationDateToAdmissionCommittee

ALTER TABLE AdmissionCommittee ADD COLUMN IF NOT EXISTS FormationDate TIMESTAMP(3) NULL;
ALTER TABLE AdmissionCommittee ALTER COLUMN approveddate DROP NOT NULL;
