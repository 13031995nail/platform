--liquibase formatted sql
--changeset nvaliuulin:likedfolder
CREATE TABLE IF NOT EXISTS public.likedfolder
(
    primarykey uuid NOT NULL,
    userid text COLLATE pg_catalog."default" NOT NULL,
    infavorites text COLLATE pg_catalog."default",
    createtime timestamp(3) without time zone,
    creator character varying(255) COLLATE pg_catalog."default",
    editor character varying(255) COLLATE pg_catalog."default",
    edittime timestamp(3) without time zone,
    CONSTRAINT likedfolder_pkey PRIMARY KEY (primarykey)
)

TABLESPACE pg_default;