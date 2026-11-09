--liquibase formatted sql
--changeset vborovin:LiquibaseAppliedScriptsFix2

update databasechangelog
set filename = replace(filename,'SQL/2024/',
                       '../../../core/backend/module/sql/main/2024/')
where filename like '%SQL/2024/%';

update databasechangelog
set filename = replace(filename,'../../../tmobis.core/SQL/2024/',
                       '../../../core/backend/module/sql/main/2024/')
where filename like '%../../../tmobis.core/SQL/2024/%';