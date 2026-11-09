--liquibase formatted sql
--changeset BorovinVA:AuditPerms

insert into storms (
   primarykey, name, type, isattribute, isoperation, isview, isclass,
   sharedoper, createtime, creator, edittime, editor, comment)
values (
   'ff60fb57-5903-47fa-984f-909343ae2fee', 'NewPlatform.Flexberry.AuditBigData.AuditRecord', null, 
   0, 0, 0, 1, 1, GETUTCDATE() at time zone 'utc',
   null, null, null, null)

insert into stormp (primarykey, subject_m0, agent_m0, createtime, creator, edittime, editor)
values ('ff60fb57-5903-47fa-984f-909343ae2fee', 'ff60fb57-5903-47fa-984f-909343ae2fee',
         '784ba5da-fe26-4872-8463-9f34fd6573b5', GETUTCDATE() at time zone 'utc', null, null, null),
   ('ff60fb57-5903-47fa-984f-909343ae2fe1', 'ff60fb57-5903-47fa-984f-909343ae2fee',
   '426f7b77-9d6d-47fb-859d-12623b85ba07', GETUTCDATE() at time zone 'utc', null, null, null),
   ('ff60fb57-5903-47fa-984f-909343ae2fe3', 'ff60fb57-5903-47fa-984f-909343ae2fee',
         '4ca8ce60-9aa8-43c3-a16b-d790b93b0066', GETUTCDATE(), null, null, null);

insert into stormac (primarykey, typeaccess, filter_m0, permition_m0, createtime, creator, edittime, editor)
values ('ff60fb57-5903-47fa-984f-909343ae2fee', 'Full', null, 'ff60fb57-5903-47fa-984f-909343ae2fee',
         GETUTCDATE() at time zone 'utc', null, null, null),
         ('ff60fb57-5903-47fa-984f-909343ae2fe1', 'Full', null, 'ff60fb57-5903-47fa-984f-909343ae2fe1',
         GETUTCDATE() at time zone 'utc', null, null, null),
         ('ff60fb57-5903-47fa-984f-909343ae2fe3', 'Full', null, 'ff60fb57-5903-47fa-984f-909343ae2fe3',
         GETUTCDATE(), null, null, null);