--liquibase formatted sql
--changeset SobakinDA:UpdateUserIsComplexPwd

UPDATE STORMAG
SET STORMAG.IsComplexPwd = Employee.IsComplexPwd
FROM STORMAG
JOIN Employee ON STORMAG.primaryKey = Employee.UserNew
WHERE Employee.UserNew IS NOT NULL;

--rollback UPDATE STORMAG SET IsComplexPwd = NULL WHERE primaryKey IN (SELECT UserNew FROM Employee WHERE UserNew IS NOT NULL);
--comment: Update STORMAG IsComplexPwd with values from Employee