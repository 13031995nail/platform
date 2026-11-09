--liquibase formatted sql
--changeset apachin:RemoveAppSetting
DROP TABLE IF EXISTS public.appsetting