--liquibase formatted sql
--changeset nvaliullin:updateVerificationRequierementIsOptional2

update VerificationRequierement set isOptional = true
where code in ('78', '79', '80', '81', '83')