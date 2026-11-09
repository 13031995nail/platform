--liquibase formatted sql
--changeset BorovinVA:NewFileProps

IF NOT EXISTS(SELECT 1 FROM sys.columns
          WHERE Name = N'ExternalId'
          AND Object_ID = Object_ID(N'File'))
BEGIN
   ALTER TABLE [File] ADD ExternalId UNIQUEIDENTIFIER NULL
END

IF NOT EXISTS(SELECT 1 FROM sys.columns
          WHERE Name = N'Name'
          AND Object_ID = Object_ID(N'File'))
BEGIN
   ALTER TABLE [File] ADD Name VARCHAR(500) NULL
END

IF NOT EXISTS(SELECT 1 FROM sys.columns
          WHERE Name = N'Size'
          AND Object_ID = Object_ID(N'File'))
BEGIN
   ALTER TABLE [File] ADD Size BigInt NULL
END