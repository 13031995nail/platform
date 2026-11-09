--liquibase formatted sql
--changeset platform:add-workobject-tso-organization

CREATE TABLE IF NOT EXISTS workobjecttsoorganization (
    primarykey uuid NOT NULL,
    workobject uuid NOT NULL,
    organization uuid NOT NULL,
    createtime timestamp without time zone,
    creator varchar(255),
    edittime timestamp without time zone,
    editor varchar(255),
    CONSTRAINT pk_workobjecttsoorganization PRIMARY KEY (primarykey),
    CONSTRAINT fk_workobjecttsoorganization_workobject
        FOREIGN KEY (workobject) REFERENCES workobject (primarykey),
    CONSTRAINT fk_workobjecttsoorganization_organization
        FOREIGN KEY (organization) REFERENCES organization (primarykey)
);

CREATE UNIQUE INDEX IF NOT EXISTS ux_workobjecttsoorganization_pair
    ON workobjecttsoorganization (workobject, organization);
CREATE INDEX IF NOT EXISTS ix_workobjecttsoorganization_workobject
    ON workobjecttsoorganization (workobject);
CREATE INDEX IF NOT EXISTS ix_workobjecttsoorganization_organization
    ON workobjecttsoorganization (organization);

--rollback DROP TABLE IF EXISTS workobjecttsoorganization;
--comment Добавлена множественная связь объектов с теплоснабжающими организациями
