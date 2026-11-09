--liquibase formatted sql
--changeset ProtasovLN:UpdateNewPasswordField

UPDATE [STORMAG]
SET [UserPwd] = [Pwd]

--rollback UPDATE [STORMAG] SET [UserPwd] = NULL;
--comment: Set new password hash field to old password hash field