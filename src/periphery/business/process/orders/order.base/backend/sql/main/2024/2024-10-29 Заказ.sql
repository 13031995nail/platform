--liquibase formatted sql
--changeset nvershinin:202410-29-order endDelimiter:$$ splitStatements:false
DO
$$
BEGIN
    CREATE TABLE IF NOT EXISTS Order (
        primaryKey UUID NOT NULL,
        Number VARCHAR(256) NOT NULL,
        Price DECIMAL(15, 2) NOT NULL,
        StartDate DATE NOT NULL,
        EndDatePlan DATE NOT NULL,
        EndDateActual DATE,
        PRIMARY KEY (primaryKey)
    );

    IF NOT EXISTS (
      SELECT 1
      FROM storms
      WHERE primarykey = 'ed55b7f8-e742-425b-a9e5-5e6c2e716755'
    ) THEN
      INSERT INTO storms (primarykey, name, type, isattribute, isoperation, isview, isclass, sharedoper, createtime, creator, edittime, editor, comment)
      VALUES ('ed55b7f8-e742-425b-a9e5-5e6c2e716755', 'Tmobis.Business.Common.Resources.Orders.Order', null, false, false, false, true, true, now() AT TIME ZONE 'utc', null, null, null, null);
    END IF;

    IF NOT EXISTS (
      SELECT 1
      FROM stormp
      WHERE primarykey = 'ed55b7f8-e742-425b-a9e5-5e6c2e716755'
    ) THEN
      INSERT INTO stormp (primarykey, subject_m0, agent_m0, createtime, creator, edittime, editor)
      VALUES ('ed55b7f8-e742-425b-a9e5-5e6c2e716755', 'ed55b7f8-e742-425b-a9e5-5e6c2e716755', '3d192c27-b628-47f2-8ef5-3b64dc766743', now() AT TIME ZONE 'utc', null, null, null);
    END IF;

    IF NOT EXISTS (
      SELECT 1
      FROM stormac
      WHERE primarykey = 'ed55b7f8-e742-425b-a9e5-5e6c2e716755'
    ) THEN
      INSERT INTO stormac (primarykey, typeaccess, filter_m0, permition_m0, createtime, creator, edittime, editor)
      VALUES ('ed55b7f8-e742-425b-a9e5-5e6c2e716755', 'Full', null, 'ed55b7f8-e742-425b-a9e5-5e6c2e716755', now() AT TIME ZONE 'utc', null, null, null);
    END IF;

    IF NOT EXISTS (
      SELECT 1
      FROM stormac
      WHERE primarykey = '306ef7e6-b516-43c6-9760-7a2cd7c17525'
    ) THEN
      INSERT INTO stormac (primarykey, typeaccess, filter_m0, permition_m0, createtime, creator, edittime, editor)
      VALUES ('306ef7e6-b516-43c6-9760-7a2cd7c17525', 'Full', null, '306ef7e6-b516-43c6-9760-7a2cd7c17525', now() AT TIME ZONE 'utc', null, null, null);
    END IF;

END;
$$ LANGUAGE PLPGSQL;
