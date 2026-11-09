--liquibase formatted sql
--changeset vborovin:0000-00-03-initial-data
insert into town (primaryKey, ADDeleteMonth, CityAcronym, CityName, Comment, CreateTime, Creator, Editor, EditTime, HourOffset, Code, Name, Actuality) values
('521fb267-b7c1-4169-b435-ae126b7938b8', null, 'ТСТ', 'Тестовый', null, now() at time zone 'utc', 'admin', null, null, 0, 'ТСТ', 'Тестовый', true);

insert into stormag (primarykey, name, login, pwd, isuser, isrole, isgroup, enabled, email, createtime, town) values 
('784ba5da-fe26-4872-8463-9f34fd6573b5', 'Пользователь', NULL, NULL, False, True, False, True, null, now() at time zone 'utc', '521fb267-b7c1-4169-b435-ae126b7938b8'),
('426f7b77-9d6d-47fb-859d-12623b85ba07', 'Администратор', NULL, NULL, False, True, False, True, null, now() at time zone 'utc', '521fb267-b7c1-4169-b435-ae126b7938b8'),
('2bef4aea-2c3a-4683-9e17-6fcb266c9f2d', 'Тестовый Тест Тестович', 'testUser1', '8562ca1bd08822f57c1fd3a4bc32cdea2f00895f', true, false, false, true, 'support@t-mobis.ru', now() at time zone 'utc', '521fb267-b7c1-4169-b435-ae126b7938b8'),
('35baa42a-a10d-464c-8469-b2ead2f6890c', 'Трофимов Трофим Трофимович', 'admin', '8562ca1bd08822f57c1fd3a4bc32cdea2f00895f', true, false, false, true, 'support@t-mobis.ru', now() at time zone 'utc', '521fb267-b7c1-4169-b435-ae126b7938b8');

insert into stormlr (primarykey, agent_m0, role_m0, createtime) values
(uuid_generate_v4(), '2bef4aea-2c3a-4683-9e17-6fcb266c9f2d', '784ba5da-fe26-4872-8463-9f34fd6573b5', now() at time zone 'utc'),
(uuid_generate_v4(), '35baa42a-a10d-464c-8469-b2ead2f6890c', '426f7b77-9d6d-47fb-859d-12623b85ba07', now() at time zone 'utc');

insert into employee (primarykey, lastname, firstname, patronymic, email, "User", createtime, actuality, town) values
(uuid_generate_v4(), 'Тестовый', 'Тест', 'Тестович', 'support@t-mobis.ru', '2bef4aea-2c3a-4683-9e17-6fcb266c9f2d', now() at time zone 'utc', true, '521fb267-b7c1-4169-b435-ae126b7938b8'),
(uuid_generate_v4(), 'Трофимов', 'Трофим', 'Трофимович', 'support@t-mobis.ru', '35baa42a-a10d-464c-8469-b2ead2f6890c', now() at time zone 'utc', true, '521fb267-b7c1-4169-b435-ae126b7938b8');