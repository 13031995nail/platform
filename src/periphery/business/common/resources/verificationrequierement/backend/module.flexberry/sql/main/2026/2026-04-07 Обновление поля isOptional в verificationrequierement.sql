--liquibase formatted sql
--changeset kmenshchikova:updateVerificationRequierementIsOptional3

update verificationrequierement 
set isOptional = true where code IN ('51','52','88','89','91','92');
update evaluationsheetrequirement set 
isOptional = true where reqorder in('51','52','88','89','91','92');