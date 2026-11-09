--liquibase formatted sql
--changeset ProtasovLN:UpdateNewPasswordField

UPDATE stormag
SET userpwd = pwd;

--rollback UPDATE stormag SET userpwd = NULL;
--comment: Set new password hash field to old password hash field