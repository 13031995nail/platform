--liquibase formatted sql
--changeset vborovin:LiquibaseAppliedScriptsFix3

update databasechangelog
set filename = replace(filename,'../../../core/backend/module/sql/main/2024/2024/', '../../../core/backend/module/sql/main/2024/')
where filename like '%../../../core/backend/module/sql/main/2024/2024/%';

update databasechangelog
set filename = replace(filename,'../../../periphery/business/common/address/workobject.base/backend/module/sql/main/2024/2024/',
                       '../../../periphery/business/common/address/workobject.base/backend/module/sql/main/2024/')
where filename like '%../../../periphery/business/common/address/workobject.base/backend/module/sql/main/2024/2024/%';