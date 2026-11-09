--liquibase formatted sql
--changeset nvershinin:202411-07-order_add_column_workobject splitStatements:false
DO
$$
BEGIN
    ALTER TABLE Order ADD COLUMN WorkObject UUID NOT NULL;

END;
$$ LANGUAGE PLPGSQL;