--liquibase formatted sql
--changeset nvaliullin:permsFixVerificationRequierement

update stormac set permition_m0 = 'cc2f43bc-7933-4397-8fcb-664b9bc310c4'
where primarykey='4872b841-9b02-432f-98de-d4e6381264b6';

update stormac set permition_m0 = 'b0ff5e6c-bfd5-4372-bb68-a6bcfe5e04e9'
where primarykey='6eb1240e-dab6-48f3-9514-aae5422b2d91';
