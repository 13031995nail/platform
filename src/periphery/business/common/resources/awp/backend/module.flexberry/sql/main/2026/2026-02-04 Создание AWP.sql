--liquibase formatted sql
--changeset ypolezhaev:2026-02-04-awp

CREATE TABLE IF NOT EXISTS Awp (
        primaryKey UUID NOT NULL,
        CreateTime TIMESTAMP(3) NULL,
        Creator VARCHAR(255) NULL,
        Editor VARCHAR(255) NULL,
        EditTime TIMESTAMP(3) NULL,
        DateBegin TIMESTAMP(3) NOT NULL,
        DateEnd TIMESTAMP(3) NOT NULL,
        IsCurrent BOOLEAN NULL DEFAULT false,
        Actuality boolean NOT NULL,
        PRIMARY KEY (primaryKey)
    );

select ChangeOrCreatePermissionFor('Full', 'admin', 'Tmobis.Business.Common.Resources.Awp');
select ChangeOrCreatePermissionFor('Read', 'Пользователь', 'Tmobis.Business.Common.Resources.Awp');