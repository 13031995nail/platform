--liquibase formatted sql
--changeset kmenshchikova:updateVerificationRequierement

update verificationrequierement 
set iscalculated = false where code='33';
update evaluationsheetrequirement set 
iscalculated = false where reqorder='33';

update verificationrequierement 
set iscalculated = true where code='10';
update evaluationsheetrequirement set 
iscalculated = true where reqorder='10';

delete from evaluationsheetrequirement where reqorder='167';
delete from verificationrequierement where code='167';
