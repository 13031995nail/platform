--liquibase formatted sql
--changeset ProtasovLN:CreateNewPasswordField

ALTER TABLE [STORMAG]
ADD [UserPwd] [varchar](256) NULL

--rollback ALTER TABLE [STORMAG] DROP COLUMN [UserPwd];
--comment: Added new field for storing password hash