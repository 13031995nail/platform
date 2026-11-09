--liquibase formatted sql
--changeset vborovin:scheduleObjCount

alter table AwpAssessingProgramEvaluationSchedule add objectsCount int null;