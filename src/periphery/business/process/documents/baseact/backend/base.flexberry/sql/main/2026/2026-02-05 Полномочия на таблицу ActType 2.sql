--liquibase formatted sql
--changeset vborovin:actTypePerms2

update storms
set name = 'Tmobis.Business.Process.Documents.Flexberry.ActType'
where name = 'Tmobis.Business.Process.Documents.ActType';