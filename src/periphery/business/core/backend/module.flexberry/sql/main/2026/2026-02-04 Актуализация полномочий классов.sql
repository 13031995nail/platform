--liquibase formatted sql
--changeset vborovin:corePermsRename

update storms
set name = replace(name, 'Tmobis.Core.Models', 'Tmobis.Business.Core.Flexberry.Models')
where name like '%Tmobis.Core.Models%';