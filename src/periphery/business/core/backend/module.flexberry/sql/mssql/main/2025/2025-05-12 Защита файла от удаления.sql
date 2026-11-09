--liquibase formatted sql
--changeset BorovinVA:FileDeleteProtection

IF NOT EXISTS(SELECT 1 FROM sys.columns
          WHERE Name = N'IsDeleteProtected'
          AND Object_ID = Object_ID(N'File'))
BEGIN
   ALTER TABLE [File] ADD IsDeleteProtected BIT NOT NULL DEFAULT 0;
   -- UPDATE [File] SET IsDeleteProtected = 0;
   -- ALTER TABLE [File] ADD DEFAULT 0 FOR IsDeleteProtected;
   -- ALTER TABLE [File] ALTER COLUMN IsDeleteProtected BIT NOT NULL;
END