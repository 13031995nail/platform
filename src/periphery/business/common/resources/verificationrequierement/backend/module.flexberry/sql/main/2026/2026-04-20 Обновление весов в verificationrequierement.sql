--liquibase formatted sql
--changeset nvaliullin:updateWeightVerificationRequierement

update verificationrequierement 
set weight = 0.05 where code='35';
update evaluationsheetrequirement 
set weight = 0.05 where reqorder='35';

update verificationrequierement 
set weight = 0.5 where code in ('27','28');
update evaluationsheetrequirement 
set weight = 0.5 where reqorder in ('27','28');

update verificationrequierement 
set weight = 0.15 where code = '25';
update evaluationsheetrequirement 
set weight = 0.15 where reqorder='25';

update verificationrequierement 
set weight = 0.01 where code = '26';
update evaluationsheetrequirement 
set weight = 0.01 where reqorder='26';