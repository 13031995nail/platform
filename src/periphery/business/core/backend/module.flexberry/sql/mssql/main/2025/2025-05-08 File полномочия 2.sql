--liquibase formatted sql
--changeset BorovinVA:FilePerms2

insert into stormp (primarykey, subject_m0, agent_m0, createtime, creator, edittime, editor)
values ('ff60fb57-5908-47fa-984f-909343ae2fe3', 'ff60fb57-5908-47fa-984f-909343ae2fee',
         '4ca8ce60-9aa8-43c3-a16b-d790b93b0066', GETUTCDATE(), null, null, null);

insert into stormac (primarykey, typeaccess, filter_m0, permition_m0, createtime, creator, edittime, editor)
values ('ff60fb57-5908-47fa-984f-909343ae2fe3', 'Full', null, 'ff60fb57-5908-47fa-984f-909343ae2fe3',
         GETUTCDATE(), null, null, null);