--liquibase formatted sql
--changeset nvaliullin:addAwpName

ALTER TABLE awp ADD COLUMN IF NOT EXISTS Name VARCHAR(150) NULL;

update awp set 
name = CONCAT(DATE_PART('year', datebegin), '-', DATE_PART('year', dateend));

