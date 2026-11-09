--liquibase formatted sql
--changeset vborovin:PassLen
alter table stormag alter column pwd type character varying(100);