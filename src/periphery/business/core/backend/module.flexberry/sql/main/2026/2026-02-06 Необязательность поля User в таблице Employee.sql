--liquibase formatted sql
--changeset nvaliullin:coreUserTypeRename

ALTER TABLE employee ALTER COLUMN "User" DROP NOT NULL