--liquibase formatted sql
--changeset BorovinVA:SessionEnd

alter table "session" add closedAt timestamp(3) without time zone null;