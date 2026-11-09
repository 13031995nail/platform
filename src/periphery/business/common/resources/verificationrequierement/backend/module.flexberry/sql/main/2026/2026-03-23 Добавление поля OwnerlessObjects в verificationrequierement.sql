--liquibase formatted sql
--changeset nvaliullin:addVerificationRequierementOwnerlessObjects

ALTER TABLE VerificationRequierement ADD COLUMN IF NOT EXISTS OwnerlessObjects BOOLEAN NOT NULL DEFAULT false;

update VerificationRequierement set OwnerlessObjects = true
where code in ('1', '2', '3', '4', '5', '6', '7', '8');

update VerificationRequierement set isOptional = true
where code in ('6');

update VerificationRequierement set iscalculated = true
where code in ('4', '5','7', '8', '102', '167')