--liquibase formatted sql
--changeset BorovinVA:SessionActuality

alter table [Session] add actuality bit not null default 1;