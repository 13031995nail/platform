--liquibase formatted sql
--changeset BorovinVA:FilePerms3

update storms
set name = 'Tmobis.Business.Core.Flexberry.Models.File'
where primarykey = 'ff60fb57-5908-47fa-984f-909343ae2fee'