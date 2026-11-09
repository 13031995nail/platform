--liquibase formatted sql
--changeset kmenshchikova:AddSignRole endDelimiter:$$ splitStatements:false
DO
$$
  BEGIN
	  IF NOT EXISTS (SELECT 1
               FROM signrole
               WHERE name = 'Председатель комиссии' or primarykey='fc83a517-d109-4c75-9ee3-a5fa52df0e93') THEN
	  INSERT INTO SignRole 
		(primarykey, name, actuality, CreateTime, Creator, Editor, EditTime) 
		VALUES 
		('fc83a517-d109-4c75-9ee3-a5fa52df0e93', 'Председатель комиссии', true, NOW(), 'admin', null, null);
	END IF;
	  
    IF NOT EXISTS (SELECT 1
                   FROM signrole
                   WHERE name = 'Представитель проверяемой организации' or primarykey='fc83a517-d109-4c75-9ee3-a5fa52df0e94') THEN
      INSERT INTO SignRole 
		(primarykey, name, actuality, CreateTime, Creator, Editor, EditTime) 
		VALUES 
		('fc83a517-d109-4c75-9ee3-a5fa52df0e94', 'Представитель проверяемой организации', true, NOW(), 'admin', null, null);
    END IF;
   
   IF NOT EXISTS (SELECT 1
                   FROM signrole
                   WHERE name = 'Член комиссии' or primarykey='fc83a517-d109-4c75-9ee3-a5fa52df0e92') THEN
      INSERT INTO SignRole 
		(primarykey, name, actuality, CreateTime, Creator, Editor, EditTime) 
		VALUES 
		('fc83a517-d109-4c75-9ee3-a5fa52df0e92', 'Член комиссии', true, NOW(), 'admin', null, null);
    END IF;
   
   IF NOT EXISTS (SELECT 1
                   FROM signrole
                   WHERE name = 'Сотрудник ТСО' or primarykey='fc83a517-d109-4c75-9ee3-a5fa52df0e95') THEN
      INSERT INTO SignRole 
		(primarykey, name, actuality, CreateTime, Creator, Editor, EditTime) 
		VALUES 
		('fc83a517-d109-4c75-9ee3-a5fa52df0e95', 'Сотрудник ТСО', true, NOW(), 'admin', null, null);
    END IF;
   
   IF NOT EXISTS (SELECT 1
                   FROM signrole
                   WHERE name = 'Заместитель председателя комиссии' or primarykey='fc83a517-d109-4c75-9ee3-a5fa52df0e96') THEN
      INSERT INTO SignRole 
		(primarykey, name, actuality, CreateTime, Creator, Editor, EditTime) 
		VALUES 
		('fc83a517-d109-4c75-9ee3-a5fa52df0e96', 'Заместитель председателя комиссии', true, NOW(), 'admin', null, null);
    END IF;
   
   IF NOT EXISTS (SELECT 1
                   FROM signrole
                   WHERE name = 'Орган, образовавший комиссию' or primarykey='fc83a517-d109-4c75-9ee3-a5fa52df0e97') THEN
      INSERT INTO SignRole 
		(primarykey, name, actuality, CreateTime, Creator, Editor, EditTime) 
		VALUES 
		('fc83a517-d109-4c75-9ee3-a5fa52df0e97', 'Орган, образовавший комиссию', true, NOW(), 'admin', null, null);
    END IF;
   END;
  
  
$$ LANGUAGE PLPGSQL;

--rollback delete from signrole where primarykey='fc83a517-d109-4c75-9ee3-a5fa52df0e97';delete from signrole where primarykey='fc83a517-d109-4c75-9ee3-a5fa52df0e93';delete from signrole where primarykey='fc83a517-d109-4c75-9ee3-a5fa52df0e94';delete from signrole where primarykey='fc83a517-d109-4c75-9ee3-a5fa52df0e92';delete from signrole where primarykey='fc83a517-d109-4c75-9ee3-a5fa52df0e95';delete from signrole where primarykey='fc83a517-d109-4c75-9ee3-a5fa52df0e96';
