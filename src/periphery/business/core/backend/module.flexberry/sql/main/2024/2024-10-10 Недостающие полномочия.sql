--liquibase formatted sql
--changeset vborovin:FixAbsentPermissions endDelimiter:$$ splitStatements:false
DO
$$
  BEGIN
    IF NOT EXISTS (SELECT 1
                   FROM storms
                   WHERE primarykey IN ('019274ed-492b-716e-91a5-0c28bbf872d4',
                                        '019274ed-492b-7777-8f04-e64644f36fc8')) THEN
      insert into storms (primarykey, name, type, isattribute, isoperation, isview, isclass, sharedoper, createtime,
                          creator, edittime, editor, comment)
      values ('019274ed-492b-716e-91a5-0c28bbf872d4', 'Tmobis.Core.Objects.User', null, false, false, false,
              true, true, now() at time zone 'utc', null, null, null, null),
             ('019274ed-492b-7777-8f04-e64644f36fc8', 'Tmobis.Core.Objects.Role', null, false, false, false,
              true, true, now() at time zone 'utc', null, null, null, null);
    END IF;

    IF NOT EXISTS (SELECT 1
                   FROM stormp
                   WHERE primarykey IN ('019274ed-492b-716e-91a5-0c28bbf872d4',
                                        '019274ed-492b-7777-8f04-e64644f36fc8',
                                        '019274f4-6fd8-72de-88a2-202275b94f17',
                                        '019274f4-6fd8-75e6-ab80-0235d50c8ab4',
                                        '019274f4-6fd8-7638-aec0-afd02ca7d035',
                                        '019274f4-6fd8-7d60-addb-79463eb2a8c4',
                                        '019274f4-6fd8-79d4-adc6-5cc8a405c8f8',
                                        '019274f4-6fd8-7519-902b-e46c84b18142',
                                        '019274f4-6fd8-7705-91ad-6f132eda7987',
                                        '019274f4-6fd8-7e0e-8e88-799f23c1ea25',
                                        '019274f8-8a75-70ce-a12b-e2523d248973',
                                        '019274f8-8a75-7ba7-a0fe-f66967d92602',
                                        '019274fa-b78c-769c-bc2d-3ab94aefd6dc',
                                        '019274fa-b78c-7e72-9fae-8c811a8c6b0b')) THEN
      insert into stormp (primarykey, subject_m0, agent_m0, createtime, creator, edittime, editor)
      values ('019274ed-492b-716e-91a5-0c28bbf872d4', '019274ed-492b-716e-91a5-0c28bbf872d4',
              '784ba5da-fe26-4872-8463-9f34fd6573b5', now() at time zone 'utc', null, null, null),
             ('019274ed-492b-7777-8f04-e64644f36fc8', '019274ed-492b-7777-8f04-e64644f36fc8',
              '784ba5da-fe26-4872-8463-9f34fd6573b5', now() at time zone 'utc', null, null, null),

             ('019274fa-b78c-769c-bc2d-3ab94aefd6dc', '019274ed-492b-716e-91a5-0c28bbf872d4',
              '426f7b77-9d6d-47fb-859d-12623b85ba07', now() at time zone 'utc', null, null, null),
             ('019274fa-b78c-7e72-9fae-8c811a8c6b0b', '019274ed-492b-7777-8f04-e64644f36fc8',
              '426f7b77-9d6d-47fb-859d-12623b85ba07', now() at time zone 'utc', null, null, null),
          
             ('019274f4-6fd8-72de-88a2-202275b94f17', '354c894a-9d0f-4e1a-b651-6c757560172f',
              '426f7b77-9d6d-47fb-859d-12623b85ba07', now() at time zone 'utc', null, null, null),
             ('019274f4-6fd8-75e6-ab80-0235d50c8ab4', 'd63fd255-fc8e-4a80-86bf-1d198725e92d',
              '426f7b77-9d6d-47fb-859d-12623b85ba07', now() at time zone 'utc', null, null, null),
             ('019274f4-6fd8-7638-aec0-afd02ca7d035', '3c99d5d5-ce86-441d-9e3a-7fd3bd2f2602',
              '426f7b77-9d6d-47fb-859d-12623b85ba07', now() at time zone 'utc', null, null, null),
             ('019274f4-6fd8-7d60-addb-79463eb2a8c4', '8830946a-7ec7-43d2-b9a6-88545b51a1dc',
              '426f7b77-9d6d-47fb-859d-12623b85ba07', now() at time zone 'utc', null, null, null),
             ('019274f4-6fd8-79d4-adc6-5cc8a405c8f8', 'de80ef44-112c-4edd-b389-6dbec3a315b6',
              '426f7b77-9d6d-47fb-859d-12623b85ba07', now() at time zone 'utc', null, null, null),
             ('019274f4-6fd8-7519-902b-e46c84b18142', '38e5b313-49cf-412d-87b8-7e6ba1a1e350',
              '426f7b77-9d6d-47fb-859d-12623b85ba07', now() at time zone 'utc', null, null, null),
             ('019274f4-6fd8-7705-91ad-6f132eda7987', 'a08723d8-93bf-40dd-9798-e8a9b61ad348',
              '426f7b77-9d6d-47fb-859d-12623b85ba07', now() at time zone 'utc', null, null, null),
             ('019274f4-6fd8-7e0e-8e88-799f23c1ea25', 'f27da4c5-e6d9-4020-9493-94a3221a4328',
              '426f7b77-9d6d-47fb-859d-12623b85ba07', now() at time zone 'utc', null, null, null),
             ('019274f8-8a75-70ce-a12b-e2523d248973', '0c8aad0b-1160-4457-855b-d5d7b013db77',
              '426f7b77-9d6d-47fb-859d-12623b85ba07', now() at time zone 'utc', null, null, null),
             ('019274f8-8a75-7ba7-a0fe-f66967d92602', '6498ba96-f56b-4d7c-aed8-ffb5251e722d',
              '426f7b77-9d6d-47fb-859d-12623b85ba07', now() at time zone 'utc', null, null, null);
    END IF;

    IF NOT EXISTS (SELECT 1
                   FROM stormac
                   WHERE primarykey IN ('019274f0-3a9d-7df0-8852-14691519a8d0',
                                        '019274f0-3a9d-777c-8fd6-6262c20fb259',
                                        '019274f5-ef56-7268-838c-4b0bfc6c989a',
                                        '019274f5-ef56-710a-8a9f-a46888841fd3',
                                        '019274f5-ef56-74d3-994f-68a5f6fae88d',
                                        '019274f5-ef56-771d-afcd-b6e19a4b1a11',
                                        '019274f5-ef56-7673-b94e-3fac07561432',
                                        '019274f5-ef56-7236-8395-d5eb5db9b46d',
                                        '019274f5-ef56-7e7c-b548-db0d1f426a4d',
                                        '019274f5-ef56-77a6-ada2-464bf81aa2c3',
                                        '019274f5-ef56-7f9e-8329-123135bf9809',
                                        '019274f8-8a75-77dc-bbe9-e65dae085de5',
                                        '019274f8-8a75-7993-989f-8964a92196ab',
                                        '019274fa-b78c-7e76-a8a3-9ff071c80d34',
                                        '019274fa-b78c-7617-b657-b0d32dc72f63')) THEN
      insert into stormac (primarykey, typeaccess, filter_m0, permition_m0, createtime, creator, edittime, editor)
      values ('019274f0-3a9d-7df0-8852-14691519a8d0', 'Read', null, '019274ed-492b-716e-91a5-0c28bbf872d4',
              now() at time zone 'utc', null, null, null),
             ('019274f0-3a9d-777c-8fd6-6262c20fb259', 'Read', null, '019274ed-492b-7777-8f04-e64644f36fc8',
              now() at time zone 'utc', null, null, null),

             ('019274fa-b78c-7e76-a8a3-9ff071c80d34', 'Full', null, '019274fa-b78c-769c-bc2d-3ab94aefd6dc',
              now() at time zone 'utc', null, null, null),
             ('019274fa-b78c-7617-b657-b0d32dc72f63', 'Full', null, '019274fa-b78c-7e72-9fae-8c811a8c6b0b',
              now() at time zone 'utc', null, null, null),
          
             ('019274f5-ef56-7268-838c-4b0bfc6c989a', 'Full', null, '019274f4-6fd8-72de-88a2-202275b94f17',
              now() at time zone 'utc', null, null, null),
             ('019274f5-ef56-710a-8a9f-a46888841fd3', 'Full', null, '019274f4-6fd8-75e6-ab80-0235d50c8ab4',
              now() at time zone 'utc', null, null, null),
             ('019274f5-ef56-74d3-994f-68a5f6fae88d', 'Full', null, '019274f4-6fd8-7638-aec0-afd02ca7d035',
              now() at time zone 'utc', null, null, null),
             ('019274f5-ef56-771d-afcd-b6e19a4b1a11', 'Full', null, '019274f4-6fd8-7d60-addb-79463eb2a8c4',
              now() at time zone 'utc', null, null, null),
             ('019274f5-ef56-7673-b94e-3fac07561432', 'Full', null, '019274f4-6fd8-79d4-adc6-5cc8a405c8f8',
              now() at time zone 'utc', null, null, null),
             ('019274f5-ef56-7236-8395-d5eb5db9b46d', 'Full', null, '019274f4-6fd8-7519-902b-e46c84b18142',
              now() at time zone 'utc', null, null, null),
             ('019274f5-ef56-7e7c-b548-db0d1f426a4d', 'Full', null, '019274f4-6fd8-7705-91ad-6f132eda7987',
              now() at time zone 'utc', null, null, null),
             ('019274f5-ef56-77a6-ada2-464bf81aa2c3', 'Full', null, '019274f4-6fd8-7e0e-8e88-799f23c1ea25',
              now() at time zone 'utc', null, null, null),
             ('019274f5-ef56-7f9e-8329-123135bf9809', 'Full', null, '019274f8-8a75-70ce-a12b-e2523d248973',
              now() at time zone 'utc', null, null, null),
             ('019274f8-8a75-77dc-bbe9-e65dae085de5', 'Full', null, '019274f8-8a75-7ba7-a0fe-f66967d92602',
              now() at time zone 'utc', null, null, null);
    END IF;
    END;
$$ LANGUAGE PLPGSQL;
