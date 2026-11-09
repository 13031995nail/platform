--liquibase formatted sql
--changeset kmenshchikova:updateVerificationRequierementIsOptional4

update verificationrequierement 
set iscalculated = true where code='87';
update evaluationsheetrequirement set 
iscalculated = true where reqorder='87';