--liquibase formatted sql
--changeset nvaliullin:insertDataOrganizationType endDelimiter:$$ splitStatements:false
DO
$$
  BEGIN
	IF NOT EXISTS (SELECT 1
				   FROM organizationtype
				   WHERE primarykey IN ('3859980c-562d-4ef3-b716-e3dc72dc6bbe')) THEN
	  INSERT INTO organizationtype (primarykey, createtime, creator, editor, edittime, code, name, actuality, shortname)
	  values ('3859980c-562d-4ef3-b716-e3dc72dc6bbe', NOW(), 'admin', null, null, null, 'Владельцы тепловых сетей, не являющиеся теплосетевыми организациями', true, 'Владельцы не ТСО');
	END IF;

	IF NOT EXISTS (SELECT 1
				   FROM organizationtype
				   WHERE primarykey IN ('1fb68a1f-7533-4822-a312-29d978168327')) THEN
	  INSERT INTO organizationtype (primarykey, createtime, creator, editor, edittime, code, name, actuality, shortname)
	  values ('1fb68a1f-7533-4822-a312-29d978168327', NOW(), 'admin', null, null, null, 'Потребители тепловой энергии, теплопотребляющие установки которых подключены (технологически присоединены) к системе теплоснабжения, приобретающие тепловую энергию (мощность), теплоноситель для использования на принадлежащих им на праве собственности или ином законном основании теплопотребляющих установках, управляющие организации, а также товарищества собственников жилья, жилищные кооперативы, жилищно-строительные кооперативы или иные специализированных потребительских кооперативов при условии осуществления ими деятельности по управлению многоквартирными домами, а также лица, с которыми в соответствии с частью 1 статьи 164 Жилищного кодекса Российской Федерации собственниками помещений в многоквартирном доме заключены договоры оказания услуг по содержанию и (или) выполнению работ по ремонту общего имущества в целях надлежащего содержания и (или) ремонта внутридомовой системы отопления в многоквартирном доме, или председатель совета многоквартирного дома в случае, если собственниками помещений в многоквартирном доме не принято решение о заключении таких договоров, или муниципальные образования в случае, если способ управления многоквартирным домом не выбран или выбранный способ управления не реализован.', true, 'Прочие');
	END IF;

	IF NOT EXISTS (SELECT 1
				   FROM organizationtype
				   WHERE primarykey IN ('44cf13f0-5b06-4509-a11d-1c05c9c39e80')) THEN
	  INSERT INTO organizationtype (primarykey, createtime, creator, editor, edittime, code, name, actuality, shortname)
	  values ('44cf13f0-5b06-4509-a11d-1c05c9c39e80', NOW(), 'admin', null, null, null, 'Теплоснабжающая, теплосетевая организация', true, 'ТСО');
	END IF;
  END;
$$ LANGUAGE PLPGSQL;