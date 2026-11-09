--liquibase formatted sql
--changeset nvaliullin:changeFieldVerificationRequierement

ALTER TABLE verificationrequierement ALTER COLUMN name TYPE VARCHAR(2000);
ALTER TABLE verificationrequierement ALTER COLUMN shortindicatorname TYPE VARCHAR(200);
ALTER TABLE verificationrequierement ALTER COLUMN supportingdocname TYPE VARCHAR(2000);
ALTER TABLE verificationrequierement ALTER COLUMN shortreqname TYPE VARCHAR(2000);
ALTER TABLE verificationrequierement ALTER COLUMN indicatorname TYPE VARCHAR(2000);