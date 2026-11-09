--liquibase formatted sql
--changeset nvaliullin:addVerificationRequierementIsOptional

ALTER TABLE VerificationRequierement ADD COLUMN IF NOT EXISTS isOptional BOOLEAN NOT NULL DEFAULT false;
