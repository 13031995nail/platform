--liquibase formatted sql
--changeset nvaliullin:addPlacesOfWorkToAdmissionCommitteeEmployee

ALTER TABLE AdmissionCommitteeEmployee ADD COLUMN IF NOT EXISTS PlacesOfWork VARCHAR(1000) NULL;
