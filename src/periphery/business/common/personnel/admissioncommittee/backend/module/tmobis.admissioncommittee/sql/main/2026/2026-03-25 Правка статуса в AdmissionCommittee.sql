--liquibase formatted sql
--changeset nvaliullin:updateStatusToAdmissionCommittee

update AdmissionCommittee set status='Утверждена'
where status='Утверждёна';