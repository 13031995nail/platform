--liquibase formatted sql
--changeset vborovin:createAwpAssessingProgram2

alter table AwpAssessingProgram add resolutionNumber varchar(10) null;
alter table AwpAssessingProgram add resolutionDate TIMESTAMP(3) null;
alter table AwpAssessingProgram add approvedDate TIMESTAMP(3) null;
alter table AwpAssessingProgram add resolutionDoc varchar null;