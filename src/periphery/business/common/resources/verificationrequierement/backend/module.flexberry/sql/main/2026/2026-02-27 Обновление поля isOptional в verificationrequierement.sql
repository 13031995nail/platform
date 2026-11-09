--liquibase formatted sql
--changeset nvaliullin:updateVerificationRequierementIsOptional

update VerificationRequierement set isOptional = true
where code in ('36', '37', '38', '40', '43','44','132','133')
