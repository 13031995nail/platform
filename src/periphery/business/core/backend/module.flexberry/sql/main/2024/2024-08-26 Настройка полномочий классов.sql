--liquibase formatted sql
--changeset apachin:PermissionsShare endDelimiter:$$ splitStatements:false
DO
$$
  BEGIN
    IF NOT EXISTS (SELECT 1
                   FROM storms
                   WHERE primarykey IN ('354c894a-9d0f-4e1a-b651-6c757560172f',
                                        'd63fd255-fc8e-4a80-86bf-1d198725e92d',
                                        '3c99d5d5-ce86-441d-9e3a-7fd3bd2f2602',
                                        '8830946a-7ec7-43d2-b9a6-88545b51a1dc',
                                        'de80ef44-112c-4edd-b389-6dbec3a315b6',
                                        '38e5b313-49cf-412d-87b8-7e6ba1a1e350',
                                        'a08723d8-93bf-40dd-9798-e8a9b61ad348',
                                        'f27da4c5-e6d9-4020-9493-94a3221a4328',
                                        '0c8aad0b-1160-4457-855b-d5d7b013db77',
                                        '6498ba96-f56b-4d7c-aed8-ffb5251e722d')) THEN
      insert into storms (primarykey, name, type, isattribute, isoperation, isview, isclass, sharedoper, createtime,
                          creator, edittime, editor, comment)
      values ('354c894a-9d0f-4e1a-b651-6c757560172f', 'Tmobis.Core.Objects.ActualObject', null, false, false, false,
              true, true, now() at time zone 'utc', null, null, null, null),
             ('d63fd255-fc8e-4a80-86bf-1d198725e92d', 'Tmobis.Core.Objects.ApplicationLog', null, false, false, false,
              true, true, now() at time zone 'utc', null, null, null, null),
             ('3c99d5d5-ce86-441d-9e3a-7fd3bd2f2602', 'Tmobis.Core.Objects.AppSetting', null, false, false, false, true,
              true, now() at time zone 'utc', null, null, null, null),
             ('8830946a-7ec7-43d2-b9a6-88545b51a1dc', 'Tmobis.Core.Objects.ChangeLog', null, false, false, false, true,
              true, now() at time zone 'utc', null, null, null, null),
             ('de80ef44-112c-4edd-b389-6dbec3a315b6', 'Tmobis.Core.Objects.Dictionary', null, false, false, false, true,
              true, now() at time zone 'utc', null, null, null, null),
             ('38e5b313-49cf-412d-87b8-7e6ba1a1e350', 'Tmobis.Core.Objects.Employee', null, false, false, false, true,
              true, now() at time zone 'utc', null, null, null, null),
             ('a08723d8-93bf-40dd-9798-e8a9b61ad348', 'Tmobis.Core.Objects.File', null, false, false, false, true, true,
              now() at time zone 'utc', null, null, null, null),
             ('f27da4c5-e6d9-4020-9493-94a3221a4328', 'Tmobis.Core.Objects.LikedFolder', null, false, false, false,
              true, true, now() at time zone 'utc', null, null, null, null),
             ('0c8aad0b-1160-4457-855b-d5d7b013db77', 'Tmobis.Core.Objects.Session', null, false, false, false, true,
              true, now() at time zone 'utc', null, null, null, null),
             ('6498ba96-f56b-4d7c-aed8-ffb5251e722d', 'Tmobis.Core.Objects.Town', null, false, false, false, true, true,
              now() at time zone 'utc', null, null, null, null);
    END IF;

    IF NOT EXISTS (SELECT 1
                   FROM stormp
                   WHERE primarykey IN ('5a641a57-4f3f-4d85-bdbc-d023d57da6ae',
                                        'e707d63b-aa83-4570-a8d1-12b8f8436176',
                                        '5094af80-347c-4e58-b76a-d3f7ee9ae482',
                                        'b9ce75a8-0c5d-4dce-9aac-ad8ecf860b44',
                                        'dd2bb301-289f-403c-ae3c-7c973c808118',
                                        '5fbb2728-c8d1-4154-897d-16e1feb0baae',
                                        'cbff39af-556d-4620-89d0-56e6b9c7f4b6',
                                        '516b45cb-89bb-4ee1-b676-692dc31949e1',
                                        '0d0734d9-e27a-4b7a-b792-19c21be1c872',
                                        'ed9beeed-28f5-4c55-8501-23a25a597d04')) THEN
      insert into stormp (primarykey, subject_m0, agent_m0, createtime, creator, edittime, editor)
      values ('5a641a57-4f3f-4d85-bdbc-d023d57da6ae', '354c894a-9d0f-4e1a-b651-6c757560172f',
              '784ba5da-fe26-4872-8463-9f34fd6573b5', now() at time zone 'utc', null, null, null),
             ('e707d63b-aa83-4570-a8d1-12b8f8436176', 'd63fd255-fc8e-4a80-86bf-1d198725e92d',
              '784ba5da-fe26-4872-8463-9f34fd6573b5', now() at time zone 'utc', null, null, null),
             ('5094af80-347c-4e58-b76a-d3f7ee9ae482', '3c99d5d5-ce86-441d-9e3a-7fd3bd2f2602',
              '784ba5da-fe26-4872-8463-9f34fd6573b5', now() at time zone 'utc', null, null, null),
             ('b9ce75a8-0c5d-4dce-9aac-ad8ecf860b44', '8830946a-7ec7-43d2-b9a6-88545b51a1dc',
              '784ba5da-fe26-4872-8463-9f34fd6573b5', now() at time zone 'utc', null, null, null),
             ('dd2bb301-289f-403c-ae3c-7c973c808118', 'de80ef44-112c-4edd-b389-6dbec3a315b6',
              '784ba5da-fe26-4872-8463-9f34fd6573b5', now() at time zone 'utc', null, null, null),
             ('5fbb2728-c8d1-4154-897d-16e1feb0baae', '38e5b313-49cf-412d-87b8-7e6ba1a1e350',
              '784ba5da-fe26-4872-8463-9f34fd6573b5', now() at time zone 'utc', null, null, null),
             ('cbff39af-556d-4620-89d0-56e6b9c7f4b6', 'a08723d8-93bf-40dd-9798-e8a9b61ad348',
              '784ba5da-fe26-4872-8463-9f34fd6573b5', now() at time zone 'utc', null, null, null),
             ('516b45cb-89bb-4ee1-b676-692dc31949e1', 'f27da4c5-e6d9-4020-9493-94a3221a4328',
              '784ba5da-fe26-4872-8463-9f34fd6573b5', now() at time zone 'utc', null, null, null),
             ('0d0734d9-e27a-4b7a-b792-19c21be1c872', '0c8aad0b-1160-4457-855b-d5d7b013db77',
              '784ba5da-fe26-4872-8463-9f34fd6573b5', now() at time zone 'utc', null, null, null),
             ('ed9beeed-28f5-4c55-8501-23a25a597d04', '6498ba96-f56b-4d7c-aed8-ffb5251e722d',
              '784ba5da-fe26-4872-8463-9f34fd6573b5', now() at time zone 'utc', null, null, null);
    END IF;

    IF NOT EXISTS (SELECT 1
                   FROM stormac
                   WHERE primarykey IN ('c3e58dff-6aa6-4a04-af9a-a25bcc19c68e',
                                        '7b06c14f-e1d8-4c5b-bcf1-69f9161fa957',
                                        'd5e6cce6-b8fe-4c0f-a2fb-ecdee7a55831',
                                        '38841d3e-2f11-49d9-9846-b55f0dc6bdb9',
                                        'bb9e56ec-ba10-416f-aab7-6401e236a5ec',
                                        '26daeac6-84e8-46ed-bc79-f206a77af2c7',
                                        '47974667-ebfd-4e0e-b9ef-9856223b6ddd',
                                        '8f9464fb-9e3e-41e1-aa27-6e3bb9870063',
                                        'fa40cc09-a702-4dba-922e-371e67a00265',
                                        '77882549-3c02-4393-a44a-793e28cf5170')) THEN
      insert into stormac (primarykey, typeaccess, filter_m0, permition_m0, createtime, creator, edittime, editor)
      values ('c3e58dff-6aa6-4a04-af9a-a25bcc19c68e', 'Read', null, '5a641a57-4f3f-4d85-bdbc-d023d57da6ae',
              now() at time zone 'utc', null, null, null),
             ('7b06c14f-e1d8-4c5b-bcf1-69f9161fa957', 'Read', null, 'e707d63b-aa83-4570-a8d1-12b8f8436176',
              now() at time zone 'utc', null, null, null),
             ('d5e6cce6-b8fe-4c0f-a2fb-ecdee7a55831', 'Read', null, '5094af80-347c-4e58-b76a-d3f7ee9ae482',
              now() at time zone 'utc', null, null, null),
             ('38841d3e-2f11-49d9-9846-b55f0dc6bdb9', 'Read', null, 'b9ce75a8-0c5d-4dce-9aac-ad8ecf860b44',
              now() at time zone 'utc', null, null, null),
             ('bb9e56ec-ba10-416f-aab7-6401e236a5ec', 'Read', null, 'dd2bb301-289f-403c-ae3c-7c973c808118',
              now() at time zone 'utc', null, null, null),
             ('26daeac6-84e8-46ed-bc79-f206a77af2c7', 'Read', null, '5fbb2728-c8d1-4154-897d-16e1feb0baae',
              now() at time zone 'utc', null, null, null),
             ('47974667-ebfd-4e0e-b9ef-9856223b6ddd', 'Read', null, 'cbff39af-556d-4620-89d0-56e6b9c7f4b6',
              now() at time zone 'utc', null, null, null),
             ('8f9464fb-9e3e-41e1-aa27-6e3bb9870063', 'Read', null, '516b45cb-89bb-4ee1-b676-692dc31949e1',
              now() at time zone 'utc', null, null, null),
             ('fa40cc09-a702-4dba-922e-371e67a00265', 'Read', null, '0d0734d9-e27a-4b7a-b792-19c21be1c872',
              now() at time zone 'utc', null, null, null),
             ('77882549-3c02-4393-a44a-793e28cf5170', 'Read', null, 'ed9beeed-28f5-4c55-8501-23a25a597d04',
              now() at time zone 'utc', null, null, null);
    END IF;

        IF NOT EXISTS (SELECT 1
                       FROM stormlr
                       WHERE primarykey = '322206ad-eacb-4429-a699-181723614ce5') THEN
            insert into stormlr (primarykey, startdate, enddate, agent_m0, role_m0, createtime, creator, edittime, editor)
            values ('322206ad-eacb-4429-a699-181723614ce5', null, null, '426f7b77-9d6d-47fb-859d-12623b85ba07',
                    '784ba5da-fe26-4872-8463-9f34fd6573b5', now() at time zone 'utc', null, null, null);
        END IF;
    END;
$$ LANGUAGE PLPGSQL;
