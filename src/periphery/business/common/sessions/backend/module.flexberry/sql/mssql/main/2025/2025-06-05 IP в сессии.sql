--liquibase formatted sql
--changeset BorovinVA:SessionIP

alter table [Session] add ip varchar(100) null;