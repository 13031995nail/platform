--liquibase formatted sql
--changeset flexberry-designer:0000-00-00-security-tables-2
IF OBJECT_ID('stormlg') IS NULL
CREATE TABLE stormlg
(
    [primaryKey] UNIQUEIDENTIFIER  NOT NULL,
    [CreateTime] DATETIME  NULL,
    [Creator] VARCHAR(255)  NULL,
    [Editor] VARCHAR(255)  NULL,
    [EditTime] DATETIME  NULL,
    group_m0 UNIQUEIDENTIFIER  NOT NULL,
    user_m0 UNIQUEIDENTIFIER  NOT NULL
);

IF OBJECT_ID('stormi') IS NULL
CREATE TABLE stormi
(
    [primaryKey] UNIQUEIDENTIFIER  NOT NULL,
    [CreateTime] DATETIME  NULL,
    [Creator] VARCHAR(255)  NULL,
    [Editor] VARCHAR(255)  NULL,
    [EditTime] DATETIME  NULL,
    user_m0 UNIQUEIDENTIFIER  NOT NULL,
    agent_m0 UNIQUEIDENTIFIER  NOT NULL
);