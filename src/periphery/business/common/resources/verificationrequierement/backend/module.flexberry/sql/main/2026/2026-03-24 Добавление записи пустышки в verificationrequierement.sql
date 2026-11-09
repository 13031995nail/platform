--liquibase formatted sql
--changeset nvaliullin:insertEmptyVerificationRequierement endDelimiter:$$ splitStatements:false
DO
$$
BEGIN
	IF NOT EXISTS (SELECT 1
			FROM verificationrequierement
			WHERE primarykey IN ('a683b9ce-3367-4fac-8604-6362a51f3204')) THEN
			INSERT INTO verificationrequierement (primarykey, createtime, creator, editor, edittime, code, name, actuality, iscalculated, 
				reqnumber, shortindicatorname, shortreqname, indicatorname, supportingdocname, weight, organizationtype)
			values ('a683b9ce-3367-4fac-8604-6362a51f3204', NOW(), 'admin', null, null, '0', '', true, true,
					'', '', '', '', 'Приложение к оценочному листу для расчета индекса готовности к отопительному периоду муниципального образования в случае наличия у них бесхозяйных объектов теплоснабжения, которым не определена организация, осуществляющая их содержание и обслуживание', null, '7909c321-f77c-4bdf-9d4d-e618fac50c81');
	END IF;     
END;
$$ LANGUAGE PLPGSQL;