--liquibase formatted sql
--changeset amavr:AddDeputyLeaderToAdmissionCommittee

ALTER TABLE AdmissionCommittee
  ADD COLUMN IF NOT EXISTS DeputyLeader UUID NULL;

ALTER TABLE AdmissionCommittee
  DROP CONSTRAINT IF EXISTS AdmissionCommittee_FDeputyLeader_0;

DROP INDEX IF EXISTS AdmissionCommittee_IDeputyLeader;

ALTER TABLE AdmissionCommittee
  ADD CONSTRAINT AdmissionCommittee_FDeputyLeader_0
  FOREIGN KEY (DeputyLeader)
  REFERENCES AdmissionCommitteeEmployee;

CREATE INDEX AdmissionCommittee_IDeputyLeader
  ON AdmissionCommittee (DeputyLeader);

--rollback DROP INDEX IF EXISTS AdmissionCommittee_IDeputyLeader; ALTER TABLE AdmissionCommittee DROP CONSTRAINT IF EXISTS AdmissionCommittee_FDeputyLeader_0; ALTER TABLE AdmissionCommittee DROP COLUMN IF EXISTS DeputyLeader;
