--liquibase formatted sql
--changeset vborovin:scheduleFixes

alter table AwpAssessingProgramEvaluationSchedule
ALTER startDate drop not null;
alter table AwpAssessingProgramEvaluationSchedule
ALTER finishDate drop not null;