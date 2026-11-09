--liquibase formatted sql
--changeset vborovin:AppLogLen
alter table ApplicationLog alter column FormattedMessage type text;
alter table ApplicationLog alter column Message type text;