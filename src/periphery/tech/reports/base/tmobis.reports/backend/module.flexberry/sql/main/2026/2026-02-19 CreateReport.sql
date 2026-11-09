--liquibase formatted sql
--changeset vborovin:createReport

CREATE TABLE IF NOT EXISTS report(
	primaryKey uuid NOT NULL,
	name character varying(255) NULL,
	description text NULL,
	fileUrl character varying(255) NULL,
	createTime timestamp(3) without time zone NULL,
	creator character varying(255) NULL,
	editTime timestamp(3) without time zone NULL,
	editor character varying(255) NULL,
	userScript text NULL,
CONSTRAINT report_pkey PRIMARY KEY (primarykey));