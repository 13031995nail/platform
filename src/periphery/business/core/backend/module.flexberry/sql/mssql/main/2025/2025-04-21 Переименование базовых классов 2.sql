--liquibase formatted sql
--changeset BorovinVA:RenameCoreNamespaces

update storms
set Name = REPLACE(Name, 'Tmobis.Core.Models.', 'Tmobis.Business.Core.Flexberry.Models.')
where Name like '%Tmobis.Core.Models.%'