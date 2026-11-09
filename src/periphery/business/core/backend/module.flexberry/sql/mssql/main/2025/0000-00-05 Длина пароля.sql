--liquibase formatted sql
--changeset vborovin:PassLen
alter table stormag alter column pwd varchar(100);