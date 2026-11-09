--liquibase formatted sql
--changeset vborovin:createAwpAssessingProgram3

alter table AwpAssessingProgram add committee uuid null;
alter table AwpAssessingProgram add FOREIGN KEY (committee) REFERENCES AdmissionCommittee 