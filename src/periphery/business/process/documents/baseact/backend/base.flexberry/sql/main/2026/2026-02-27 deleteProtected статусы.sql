--liquibase formatted sql
--changeset vborovin:deleteProtected2

update actstatus
set deleteprotected = true
where primarykey in ('119719b2-e3bd-4f00-a051-2426220d3053', '119719b2-e3bd-4f00-a051-2426220d3054');