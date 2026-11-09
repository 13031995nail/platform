--liquibase formatted sql
--changeset SobakinDA:AddIsComplexPwdToUser

IF NOT EXISTS(SELECT 1 FROM sys.columns
          WHERE Name = N'IsComplexPwd'
          AND Object_ID = Object_ID(N'STORMAG'))
BEGIN
   ALTER TABLE STORMAG ADD IsComplexPwd BIT NULL
END

--rollback ALTER TABLE STORMAG DROP COLUMN IF EXISTS IsComplexPwd;
--comment: Adding password complexity check to the user
