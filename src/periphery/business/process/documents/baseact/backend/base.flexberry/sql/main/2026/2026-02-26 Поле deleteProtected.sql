--liquibase formatted sql
--changeset vborovin:deleteProtected

alter table actstatus add column if not exists deleteprotected boolean not null DEFAULT false;