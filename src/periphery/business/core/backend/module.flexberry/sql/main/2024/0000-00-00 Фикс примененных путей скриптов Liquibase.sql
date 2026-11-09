--liquibase formatted sql
--changeset vborovin:LiquibaseAppliedScriptsFix

update databasechangelog
set filename = replace(filename,'../../../core/backend/module/sql/main/', '../../../core/backend/module/sql/main/2024/')
where filename like '%../../../core/backend/module/sql/main/%';

update databasechangelog
set filename = replace(filename,'../../../periphery/business/common/address/workobject.base/backend/module/sql/main/',
                       '../../../periphery/business/common/address/workobject.base/backend/module/sql/main/2024/')
where filename like '%../../../periphery/business/common/address/workobject.base/backend/module/sql/main/%';