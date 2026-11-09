--liquibase formatted sql
--changeset BorovinVA:SessionEnd

alter table [Session] add closedAt datetime null;