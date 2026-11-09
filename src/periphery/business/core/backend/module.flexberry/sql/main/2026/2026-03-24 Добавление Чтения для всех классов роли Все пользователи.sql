--liquibase formatted sql
--changeset MenshchikovaKA:addRightsToRoleAllUsers endDelimiter:$$ splitStatements:false
	
DO $$
DECLARE
    rec RECORD;  -- Переменная для хранения текущей строки
BEGIN
    -- Цикл по всем строкам таблицы
    FOR rec IN SELECT * FROM storms LOOP
        PERFORM ChangeOrCreatePermissionFor('Read', 'Все пользователи', rec.name);

    END LOOP;
END $$;