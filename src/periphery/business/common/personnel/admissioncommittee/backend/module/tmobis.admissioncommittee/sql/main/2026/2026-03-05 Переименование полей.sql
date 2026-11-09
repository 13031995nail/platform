--liquibase formatted sql
--changeset nvaliullin:alterColumnToAdmissionCommittee

ALTER TABLE AdmissionCommittee RENAME COLUMN approveddate TO regdate;
ALTER TABLE AdmissionCommittee RENAME COLUMN formationdate TO approveddate;
ALTER TABLE AdmissionCommittee ALTER COLUMN regdate SET NOT NULL;
