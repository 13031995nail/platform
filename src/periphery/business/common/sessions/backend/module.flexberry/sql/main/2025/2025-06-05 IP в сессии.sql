--liquibase formatted sql
--changeset BorovinVA:SessionIP

alter table "session" add ip varchar(100) null;