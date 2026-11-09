--liquibase formatted sql
--changeset BorovinVA:SessionActuality

alter table "session" add actuality boolean not null default true;