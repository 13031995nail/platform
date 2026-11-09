--liquibase formatted sql
--changeset BorovinVA:RenameCoreNamespaces

update storms
set Name = REPLACE(Name, '.Objects.', '.Models.')
where Name like '%Tmobis.Core%'