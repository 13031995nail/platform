--liquibase formatted sql
--changeset ProtasovLN:CreateNewPasswordField

ALTER TABLE stormag
ADD userpwd VARCHAR(256) NULL;

--rollback ALTER TABLE stormag DROP COLUMN userpwd;
--comment: Added new field for storing password hash