--liquibase formatted sql
--changeset BorovinVA:NewFileProps

ALTER TABLE "file" ADD ExternalId uuid NULL;
ALTER TABLE "file" ADD Name VARCHAR(500) NULL;
ALTER TABLE "file" ADD Size bigint NULL;