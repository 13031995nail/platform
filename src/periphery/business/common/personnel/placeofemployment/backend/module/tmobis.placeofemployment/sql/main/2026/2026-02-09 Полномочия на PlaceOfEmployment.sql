--liquibase formatted sql
--changeset nvaliullin:permsPlaceOfEmployment

select ChangeOrCreatePermissionFor('Full', 'admin', 'Tmobis.Business.Common.Personnel.PlaceOfEmployment');
select ChangeOrCreatePermissionFor('Full', 'Пользователь', 'Tmobis.Business.Common.Personnel.PlaceOfEmployment');