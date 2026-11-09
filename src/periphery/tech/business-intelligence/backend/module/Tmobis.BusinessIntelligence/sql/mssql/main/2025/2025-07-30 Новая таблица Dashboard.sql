--liquibase formatted sql
--changeset ProtasovLN:Dashboards

CREATE TABLE [Dashboard] (
    [primaryKey] UNIQUEIDENTIFIER  NOT NULL,
    [CreateTime] DATETIME  NULL,
    [Creator] VARCHAR(255)  NULL,
    [Editor] VARCHAR(255)  NULL,
    [EditTime] DATETIME  NULL,
    [MetabaseDashboardId] nvarchar(255),
    [Note] nvarchar(max),
    [TmobisDashboardId] nvarchar(255)
);

IF NOT EXISTS (
  SELECT 1
  FROM storms
  WHERE primarykey IN ('287E464A-53C4-4AF9-9D73-2155551C4E22')
)
BEGIN
  INSERT INTO storms (primarykey, name, type, isattribute, isoperation, isview, isclass, sharedoper, createtime, creator, edittime, editor, comment) 
  VALUES ('287E464A-53C4-4AF9-9D73-2155551C4E22', 'Tmobis.BusinessIntelligence.Dashboard', NULL, 0, 0, 0, 1, 1, GETUTCDATE(), NULL, NULL, NULL, NULL);
END;

IF NOT EXISTS (
  SELECT 1
  FROM stormp
  WHERE primarykey IN ('287E464A-53C4-4AF9-9D73-2155551C4E22')
)
BEGIN
  INSERT INTO stormp (primarykey, subject_m0, agent_m0, createtime, creator, edittime, editor) 
  VALUES ('287E464A-53C4-4AF9-9D73-2155551C4E22', '287E464A-53C4-4AF9-9D73-2155551C4E22', '784ba5da-fe26-4872-8463-9f34fd6573b5', GETUTCDATE(), NULL, NULL, NULL);
END;

IF NOT EXISTS (
  SELECT 1
  FROM stormac
  WHERE primarykey IN ('287E464A-53C4-4AF9-9D73-2155551C4E22')
)
BEGIN
  INSERT INTO stormac (primarykey, typeaccess, filter_m0, permition_m0, createtime, creator, edittime, editor) 
  VALUES ('287E464A-53C4-4AF9-9D73-2155551C4E22', 'Full', NULL, '287E464A-53C4-4AF9-9D73-2155551C4E22', GETUTCDATE(), NULL, NULL, NULL);
END;

--rollback DROP TABLE Dashboard;
--comment: Added new table for storing dashboard metadata