--liquibase formatted sql
--changeset ProtasovLN:Dashboards endDelimiter:$$ splitStatements:false
DO
$$
BEGIN
	CREATE TABLE IF NOT EXISTS dashboard (    
	  primaryKey UUID NOT NULL,
	  createTime TIMESTAMP(3) NULL,
	  creator VARCHAR(255) NULL,
	  editor VARCHAR(255) NULL,
	  editTime TIMESTAMP(3) NULL,
	  metabaseDashboardId varchar(255) NOT NULL,
	  note text NULL,
	  tmobisDashboardId varchar(255) NOT NULL,
		Code varchar(10) NULL,
		Name varchar(200) NULL,
		Actuality boolean NOT NULL
	);
	
	IF NOT EXISTS (
	  SELECT 1
	  FROM storms
	  WHERE primarykey IN ('287E464A-53C4-4AF9-9D73-2155551C4E22')
	) THEN
	  insert into storms (primarykey, name, type, isattribute, isoperation, isview, isclass, sharedoper, createtime, creator, edittime, editor, comment) values
	    ('287E464A-53C4-4AF9-9D73-2155551C4E22', 'Tmobis.BusinessIntelligence.Dashboard', null, false, false, false, true, true, now() at time zone 'utc', null, null, null, null);
	END IF;
	
	IF NOT EXISTS (
	  SELECT 1
	  FROM stormp
	  WHERE primarykey IN ('287E464A-53C4-4AF9-9D73-2155551C4E22')
	) THEN
	  insert into stormp (primarykey, subject_m0, agent_m0, createtime, creator, edittime, editor) values
	    ('287E464A-53C4-4AF9-9D73-2155551C4E22', '287E464A-53C4-4AF9-9D73-2155551C4E22', '784ba5da-fe26-4872-8463-9f34fd6573b5', now() at time zone 'utc', null, null, null);
	END IF;
	
	IF NOT EXISTS (
	  SELECT 1
	  FROM stormac
	  WHERE primarykey IN ('287E464A-53C4-4AF9-9D73-2155551C4E22')
	) THEN
	  insert into stormac (primarykey, typeaccess, filter_m0, permition_m0, createtime, creator, edittime, editor) values
	    ('287E464A-53C4-4AF9-9D73-2155551C4E22', 'Full', null, '287E464A-53C4-4AF9-9D73-2155551C4E22', now() at time zone 'utc', null, null, null);
	END IF;

END;
$$ LANGUAGE PLPGSQL;

--rollback DROP TABLE dashboard;
--comment: Added new table for storing dashboard metadata
