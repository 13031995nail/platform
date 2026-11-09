--liquibase formatted sql
--changeset flexberry-designer:0000-00-04-security-tables-2 endDelimiter:$$
CREATE TABLE IF NOT EXISTS public.stormlg
(
    primarykey uuid NOT NULL,
    group_m0 uuid NOT NULL,
    user_m0 uuid NOT NULL,
    createtime timestamp(3) without time zone,
    creator character varying(255) COLLATE pg_catalog."default",
    edittime timestamp(3) without time zone,
    editor character varying(255) COLLATE pg_catalog."default",
    CONSTRAINT stormlg_pkey PRIMARY KEY (primarykey)
)

TABLESPACE pg_default;

CREATE TABLE IF NOT EXISTS public.stormi
(
    primarykey uuid NOT NULL,
    user_m0 uuid NOT NULL,
    agent_m0 uuid NOT NULL,
    createtime timestamp(3) without time zone,
    creator character varying(255) COLLATE pg_catalog."default",
    edittime timestamp(3) without time zone,
    editor character varying(255) COLLATE pg_catalog."default",
    CONSTRAINT stormi_pkey PRIMARY KEY (primarykey)
)

TABLESPACE pg_default;