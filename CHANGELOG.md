## [4.5.2](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.5.2) (2026-10-12)

### Изменения

- **documents:** интегрировать документы в ролевые пространства  ([172f639](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/172f639c692a3bc4291d3f3b2b58df584eab7645))

### Документация

- описание модулей платформы ([66cea86](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/66cea868d603d47eda2829d43471645179f22f1b)), [#836](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/836)
- перенос технической части документации из Outline ([06e87ce](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/06e87ceb129cf97f8443d5aea77a528af1870fdc)), [#836](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/836)
- перенос технической части документации из Outline ([713957b](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/713957bb5983d79a5449013c6537a97ecec49722)), [#836](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/836)

## [4.5.1](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.5.1) (2026-10-05)

### Изменения

- **portal:** карточка объекта, требования, состояния, прогресс и замечания ([08e8214](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/08e8214baa98bacc09d19a20048393f9a2b07ec5))
- **workshift:** перенос смен на будущие даты   ([7093875](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/7093875a189bc3064f4a9debe0fd2af1191e48db)), [timepath/platform/tmobis/tmobis.core#781](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/781)

### Исправления ошибок

- **work-object:** исправить ограничения доступа и открытие аудита  ([7e16776](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/7e167762eddcb27d5da48d8f5541ba2233ffca04))

## [4.5.0](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.5.0) (2026-09-18)

### ⚠ КРИТИЧЕСКИЕ ИЗМЕНЕНИЯ

* **back,di:** удален IRepository\<T\>, часть бизнес-логики может перестать работать

### Изменения

- адаптация существующей бизнес-логику в новый формат ([98f69fc](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/98f69fc8cd2a11dd141b2c74f77ff58432b9982c)), [#625](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/625)
- базовая списковая форма на React Native ([7268c62](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/7268c62d2ba4a87836d108f45c32cea4cfc60599)), [#575](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/575)
- добавлен лукап ([6116531](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/6116531a8bf6a1d377eb94ca89a3daa7426f6ac8)), [#607](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/607)
- добавлен s3-адаптер для файлового хранилища ([0c74611](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/0c746118c7dbd4a6f73523089220b679e256bfb1)), [#589](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/589)
- запоминание пейджинга в параметрах url-запроса ([22b776c](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/22b776c74a06f22f92ca4bbff426c1a8e15bc14f)), [#638](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/638)
- кастомизация сайдбара Expo ([7c3e1a0](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/7c3e1a0595a13b7744801c58be622ab921e5de87)), [#617](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/617)
- локализация и база системы модулей ([6f24b36](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/6f24b3600b752a3298a595ff3293535a80e813d0)), [#661](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/661)
- перенос компонентов Gluestack в ядро ([0b8dd91](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/0b8dd9145efba3f4f2b601eae2f080d845b60c22)), [#651](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/651)
- **access,2234:** добавлены доменные ограничения доступа к объектам ([917c33c](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/917c33cada85e0e938b9ea92acbc891c1a910fe8))
- **access:** добавить доменные ограничения доступа к объектам ([f86fd02](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/f86fd02259ed667aa56268c06f1845a5468cd9c5)), [#302](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/302)
- **audit:** система аудита ([821097f](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/821097fa450baaac1e3de774af7242dbcc531579)), [#686](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/686)
- **auth,back:** новая версия ApiKeyAuth, совместимая с MinimalAPI ([f2fcead](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/f2fcead6ee489802c16535061315533c305f08d2)), [#775](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/775)
- **auth:** кейклок для натива ([40cda3f](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/40cda3ff90ba5cd7396a8e22d525c315c39f7106)), [#690](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/690)
- **auth:** настройка целевого сценария аутентификации и авторизации в примере EF+React ([05a3dbc](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/05a3dbcbca380372a864cb7de590d751b3e55c64)), [#777](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/777)
- **back,api:** обработка query-сообщений через WebAPI ([8f090a0](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/8f090a0c77018ebe9658432a148af7a1a5e60ce9)), [#754](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/754)
- **back,auth,roles:** получение ролей из клеймов текущего пользователя ([838b75e](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/838b75e1e365f22493577887d7e3ed413c5accc6)), [#779](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/779)
- **back,business:** модель рабочих смен на бэкенде  ([0453b34](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/0453b34185917f4ce90ca7fa50b408d4cdc6281e)), [#741](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/741)
- **back,di:** использование автоскоупа из HTTP запросов для репозиториев ([d0c8f61](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/d0c8f61047f7d274ec14c3d2d5cc45f643979e1f)), [#776](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/776)
- **back,di:** перевод использования репозиториев на scoped lifetime ([ca29300](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/ca293008bee59c4d818703daad6c2eb27c915058)), [#758](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/758)
- **back,entities:** вычислимые поля при генерации сущностей и проверка работы иерархической бизнес-логики ([339b69f](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/339b69f8f4468194842c881c14ce4df61f9e1261)), [#738](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/738)
- **back,entities:** генерация сущностей из интерфейсов в прикладной проект ([806fa9a](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/806fa9a156a3d2b192a7cac498a0ec60ed851b77)), [#738](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/738)
- **back,entities:** ридми для генератора сущностей ([be74682](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/be74682be6cb8f83cbde48660323ec4d5f84ac24)), [#738](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/738)
- **back,file:** универсальный контроллер для получения файлов ([61c8aa7](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/61c8aa79085e9b2696d1b8db8269b7aae1e01e4a)), [#685](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/685)
- **back,flexberry:** реализован транзакционный FlexberryRepository ([5c77e54](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/5c77e547f2649110978bf3ff9fe1bd4c902cd9f0)), [#628](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/628)
- **back,notifications:** модель и сервис отправки уведомлений ([63fc871](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/63fc871cc5d40aabd3b0201ab16cb374b8cc74b9)), [#727](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/727) [#725](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/725)
- **back:** добавлена авторизация в Swagger ([02a1a2f](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/02a1a2fb7ec1ab6e8ff378e873b2ed386ae8b471)), [#786](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/786)
- **back:** добавлено логирование в сэмпл бэкенда ([2f64d27](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/2f64d27ded2096313024535913aae86156c70d46))
- **back:** модуль-адаптер MAX ([dab5490](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/dab5490a7b255f80cf86c47fc6921f59a22a279c)), [#804](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/804)
- **backend:** реализовать поиск в запросе списка на бэкенде ([b432d7d](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/b432d7d147c59733828b9fa8e504f5baa3410ba4)), [#637](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/637)
- **backend:** реализовать фильтрацию записей на стороне бэкенда при запросах списка сущностей ([5aa643e](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/5aa643e952c7be471732ef2733629870a50f122f)), [#642](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/642)
- **business,workshift:** расширить статусы рабочих смен ([66d7c13](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/66d7c13af05f4e4cde07b3cb2c521437be78f8e1)), [#780](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/780)
- **deploy:** Развернуть стенд для React + EF ([97f73c8](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/97f73c84f19a207364a66d5c2acdbb5d931e34da)), [#688](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/688)
- **document:** создана роль userAdmin ([ee71983](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/ee71983606c005637352b159e4d1c1f012335d45)), [#550](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/550)
- **ef,back:** реворк EFRepository ([c1f1c65](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/c1f1c655a63d57374c7a713a32d56c63bdf1eaa7)), [#805](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/805)
- **ef:** ограничения по филиалу для EF ([5975afc](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/5975afc2751c3a104ae441a2f6ca087d452b56e6)), [#757](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/757)
- **expo-app:** добавить PWA-манифест  ([fc1abb4](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/fc1abb4986e50bb2d94979fcb16bf41d29e87a93)), [#814](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/814)
- **expo:** компонент дропдауна на expo + gluestack  ([78a597b](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/78a597bb14a8146763d3fb52e134e2caf617a6a5)), [#610](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/610)
- **expo:** реализовать контекстное меню для строк списка ([9580f05](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/9580f05d2c25244d5203294428d503ca123e1388)), [#631](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/631)
- **expo:** реализовать форму редактирования на Expo ([e204529](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/e20452940272d0341c0e3f07d2c1148149a1128c)), [#577](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/577)
- **files,react,back:** соединение файлового компонента с файловым контроллером ([ada5b03](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/ada5b035cffec496f0a0628526e81664ca602c36)), [#721](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/721)
- **flexberry,security:** исправление уязвимости утечки данных через odata ([088dd69](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/088dd69cfd49a9f9a72b3fbeab32a7bc5b89a8be)), [#547](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/547)
- **flexberry,security:** исправление уязвимости эскалации привилегий через изменение пользователя ([e0ff65c](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/e0ff65ca87086a37df39514c5b55710830688979)), [#549](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/549)
- **front,business:** модель рабочих смен на фронте ([7bd014c](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/7bd014cd03a84dc45618c630e64741a1781e0b67)), [#742](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/742)
- **front:** получение текущих координат в React Native ([35b89fd](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/35b89fd50fa27d3014586c5e5e5cdabc2882dabd)), [#755](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/755)
- **keycloak,back:** модуль аутентификации и авторизации через Keycloak для бэкенда ([3ee2405](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/3ee24059d43aa972e8c2c0e0065f755d675a8a6d)), [#595](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/595)
- **logging,react:** модуль логирования для фронтенда React ([4fa9c0d](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/4fa9c0d1d280f2fc6086b4b0069ea2fd50301bbf)), [#813](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/813)
- **medium,back,react,business:** управление транзакциями в Medium ([33c1ddf](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/33c1ddfd1504adc9ecb948344c7b6e691416c4f6)), [#787](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/787)
- **messenger,notification:** интеграции с Telegram, VK, Mattermost, Yandex Messenger ([be5dfb0](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/be5dfb04396c814d40be32af544cec32598365e2))
- **meters,ai:** модуль распознавания показаний ([637c876](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/637c8765445beb9495fd796977f25d8c97f9e4aa)), [#778](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/778)
- **mssql,back,ef:** модуль работы EF с СУБД MSSQL ([78d2e0b](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/78d2e0bbdbed19e76c63f385a64409151ecac937)), [#726](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/726)
- **notification:** уведомления через WebSocket на бэкенде ([e016285](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/e0162858b620036eaa393df0fb55fa63a0d026b8)), [#728](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/728)
- **notifications:** список уведомлений и индикатор непрочитанных ([45eeda9](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/45eeda942ebd36195d4d0c0c010e506dfab0b639)), [#734](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/734)
- **offline,ember:** добавлена загрузка json данных в ЛБД пользователя и обратно ([83acd89](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/83acd89f965ca6e5719689cc6c71e57fa52f3c3e)), [#554](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/554)
- **react,auth:** получение роли пользователя из сессии в React ([ab81553](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/ab815537254a653867a716c27d98572d17b0854e)), [#826](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/826)
- **react,back,business:** формы рабочих смен  ([bbf375e](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/bbf375e8905ec4d84182b35188d19ecce59f640b)), [#743](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/743)
- **react,back,entity:** добавлен подсчет записей для пагинации ([0f806c9](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/0f806c94d0d1193b9fc7d797699527d17d652dfa)), [#644](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/644)
- **react,back,medium:** база слоя обмена сообщениями для React + SignalR ([a2cacba](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/a2cacbab15efe3979dd63e37721df0b72f4fb33d)), [#731](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/731)
- **react,back:** фильтрация списка по периоду дат ([f6a9e81](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/f6a9e81bfdb6ac2e4be137b076a457d544fe6948)), [#744](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/744)
- **react,files:** реализован файловый компонент ([6c8d91e](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/6c8d91ee5831a0187d1c96267e0acae952912ced)), [#611](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/611)
- **react,keycloak:** фронтенд аутентификация через Keycloak ([0b794fc](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/0b794fce61d8f497faedf463cc194d04d26321d1)), [#690](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/690)
- **react,medium:** добавлен клиент WebAPI-сообщений для React ([b9d0d30](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/b9d0d309e060842d82cd487f7d0cea040a851af0)), [#753](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/753)
- **react,notifications:** модель уведомлений на фронтенде  ([c00ca4b](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/c00ca4bff1c47de104b18722fa2ea66ac4b0d0b6)), [#729](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/729)
- **react,notifications:** модуль WebSocket уведомлений для фронтенда ([58d9a9a](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/58d9a9a51ebaf01be147f0094892a025699d8e64)), [#730](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/730)
- **react,workshift:** рабочие смены, фронтовые бизнес-доработки   ([e0af816](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/e0af8166ee984549065f8aa69ec53ecf84d6fab2)), [#800](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/800)
- **react:** возможность добавлять кастомные компоненты в колонки списка с примером вывода пунктов на списке в смене ([1ab5992](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/1ab59927326c222a8392cb36588550bd3cdf4d6e)), [#750](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/750) [#749](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/749)
- **react:** возможность доопределения форм редактирования ([e5447f6](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/e5447f68c12abd83bd3e00c25a8eb3a4ec399cde)), [#739](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/739)
- **react:** динамические табы на форме редактирования ([90680b3](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/90680b315c93a6be21f724d10ae334575718c1ce)), [#798](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/798)
- **react:** добавить мобильное представление списковой формы ([df21749](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/df21749f472e9bf86b8aeca36bbddb02a249472c))
- **react:** добавлен дейтпикер для React с выбором даты, времени и ограничениями min max ([bbe8531](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/bbe8531bc6a5e497b5c7267e82e2793962f96265)), [#676](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/676)
- **react:** добавлен компонент TmobisDatetime ([0b6d039](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/0b6d039ec9abe3fd0b685d509f5748fc9722e802))
- **react:** добавлен компонент TmobisDropdown  ([c06f24f](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/c06f24f98d84ee99dd0602609c64b36e1cbdc484)), [#696](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/696)
- **react:** добавлен компонент TmobisLookup ([92b410f](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/92b410f1dfd20d5467a956b8a1c547a4a9de64e1))
- **react:** добавлен компонент TmobisRegexInput ([63a2fa8](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/63a2fa852821880f0e14c0e94567294a0d9099ae))
- **react:** добавлен компонент TmobisSwitch ([c26f018](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/c26f0180fd1c1ece4cd0f5cda0c1c03b9a1e446c))
- **react:** добавлен мультивыбор в лукапы ([3b1d6de](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/3b1d6de8769ac9886276cb12b5b6fc476f05ca8b)), [#747](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/747)
- **react:** добавлен пример работы с таймером для выполнения фоновых задач ([1964624](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/19646240a816d888e653721102f89f62494e8a29)), [#580](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/580)
- **react:** добавлен таб бар на Expo и GlueStack   ([ba9a23c](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/ba9a23c00d08e2eb4aefff361f1b8113d36a17a9)), [#616](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/616)
- **react:** добавлен чекбокс для форм Expo/GlueStack ([f4766ef](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/f4766ef3c6a760e512ef56c3793995f3d21dc245)), [#677](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/677)
- **react:** добавлен IEntity в React ([8549264](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/8549264198925a6304734f82da33607085ed4dc8)), [#722](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/722)
- **react:** добавлено модальное окно подтверждения выхода с несохраненными изменениями ([138ae99](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/138ae990fd2175d4369d5b48d3723bb2277b504b)), [#692](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/692)
- **react:** доопределение колонок списковой формы в модулях  ([5120902](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/512090221f8204cfa3abc4bbc9e45a57362b1654)), [#799](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/799)
- **react:** изменен компонент TmobisDropdown   ([2f7a4c4](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/2f7a4c4ba4818175091a2f689772ef5c65d958b6)), [#795](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/795)
- **react:** изменены стили TmobisButton ([10e2616](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/10e26165c0a48cf7c5e36d8a62aafa37840ef7e8))
- **react:** изменены стили TmobisModal ([8618509](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/861850909bc53cf2e9f7ef925e7a0d9088698789))
- **react:** компонент TmobisButton ([31de9ed](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/31de9ed0964e33256525f2141d44f34ab87cdd8e)), [#655](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/655)
- **react:** компонент TmobisInput ([a8ee143](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/a8ee143d056ae157392ddc8f2d09cbf49a814250))
- **react:** компонент Toast для уведомлений ([a0fd996](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/a0fd996f209f29e1c94ea24aa2ab0e0f31eb6f13)), [#732](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/732)
- **react:** мобильная верстка формы редактирования   ([7d7f42d](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/7d7f42dd32c97e13c5592d5e7993850ceb3c0fb0)), [#833](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/833)
- **react:** новый хедер приложения   ([5f8d76d](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/5f8d76d1a2ee1c3d76c05ecf45a5d371605e85b3)), [#818](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/818)
- **react:** обновлены переменные темы приложения  ([3b69330](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/3b693304066d25793aba145b980511a1edcc42f1)), [#693](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/693)
- **react:** оптимизация сериализации ключей запроса у диспетчера сообщений на фронте   ([e1c001c](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/e1c001c8e6a660314d8f92318419313e02f610c8)), [#862](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/862)
- **react:** поиск на списковой форме и чистка списковой формы ([4cca63b](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/4cca63b8299dd32dd984b4b534a70ef4aedeca06)), [#634](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/634)
- **react:** приведена в порядок списковая форма   ([fc617ef](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/fc617efe4312d0dd28a14d89a04f4f87a4e91d40)), [#821](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/821)
- **react:** пример формы на Expo + GlueStack   ([1a093c6](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/1a093c6e5d8ee9c19f041bcf91a73e2d83014376)), [#593](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/593)
- **react:** реализация экрана настроек ([26df304](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/26df304340a2003e20577205883f52fe269e66af)), [#817](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/817)
- **react:** реализован кастомный сайдбар для операций ([ae391d6](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/ae391d694f0e29ff35d41ae5a0c73e6488c2d2ad)), [#681](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/681)
- **react:** реализован компонент спойлера ([7590a8d](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/7590a8dea267b95393844901b6a83a6fac545fe7)), [#622](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/622)
- **react:** реализован компонент TmobisAlert ([9b86bbd](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/9b86bbd1f626b4fb015fa6bdb1708ce2a182e47d)), [#603](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/603)
- **react:** реализован компонент TmobisModal ([ccd1946](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/ccd19465b85b3e3c139e05e698d5de0ae6ccecdf)), [#603](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/603)
- **react:** реализован хедер форм ([145d6e8](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/145d6e8b2dcb602c0447961849334ad932fc8602)), [#682](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/682)
- **react:** реализовать сортировку списка по колонке ([4261deb](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/4261deb3b4ed516d53357818a27f566de621472e)), [#643](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/643)
- **react:** реализовать текстовый компонент с регулярными выражениями ([61a2639](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/61a26390c5567f72aa10e1e426370e3bf46cb44a)), [#683](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/683)
- **react:** сайдбар в новом дизайне   ([c0f5037](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/c0f5037b9af8276daaead2045608acdd9ebb063c)), [#819](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/819)
- **react:** система состояния формы ([0d6d3d3](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/0d6d3d3f14c5a05895409276a6a676d24068e0eb)), [#723](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/723)
- **react:** стилизация компонента Tabs   ([22d257a](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/22d257a351e6c7f63cc6bd95a480c233d289f8e2)), [#702](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/702)
- **react:** стилизация компонента Toggler ([61aaa15](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/61aaa156aee05075e5e39efd93384a28d6172927)), [#700](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/700)
- **react:** стилизация компонентов Badge и Chip   ([c5b63f8](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/c5b63f8bfa0dee0bb0f6215bdda50c4855fe25d9)), [#701](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/701)
- **react:** улучшения формы редактирования ([6174aec](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/6174aec53e7a5f138176d8ea091ea05d18774b35)), [#689](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/689)
- **react:** фильтры на списковых формах ([a41a956](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/a41a956ba71344b353a83a6b9fc9da75d8e4bccb))
- **react:** хранение текущих фильтров в url параметрах ([a791854](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/a791854fb069a43426c69185b8a5f3e9451e9675)), [#641](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/641)
- **react:** экран рабочего стола ([01246c6](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/01246c63fe9f6a2519c24a1aacc210d741b32453)), [#816](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/816)
- **reports:** форма просмотра и скачивания ПФ ([1726fba](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/1726fbac43304c084832f82052260fbb32d4d895)), [#811](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/811)
- **validation:** добавлена валидация форм редактирования ([8a07ad4](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/8a07ad45f90a829655f24e29960e469beb72c580)), [#691](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/691)
- **work:** рабочие смены фиксация работ на фронтенде ([7832c45](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/7832c45cd23c8c5d76314ce42d1d82f520729bf2)), [#751](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/751)
- **workshift:** раздел "Мои задачи" и сообщение GetMyWorkShiftId   ([dc30aad](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/dc30aad0732c18814639d84cb68d2ba95b32ba66)), [#756](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/756)
- **workshift:** фиксация работ на бэкенде ([20155db](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/20155db79733cf62fd16b46cd7269267d4522c08)), [#752](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/752)
- **workshift:** фильтры на списковой форме смен   ([eef6ded](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/eef6dedc83e8e6b5d656051c926b41a74f7a48dc)), [#745](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/745)

### Исправления ошибок

- использование @tmobis/core/ вместо @/ в пакете ядра ([ea25d85](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/ea25d850b78206cb31e5564c1c5b29935688870a)), [#717](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/717)
- исправление ошибок Drawer ([595ea5b](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/595ea5b049f1c939d2524f5984689b1bb64d60c5))
- исправлено создание новой парольной записи пользователя ([513d4b9](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/513d4b9d9eb26760f27381a2e467d686b705cfe2))
- **back:** восстановление дефолтных моделей на бэкенде ([3147d50](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/3147d504f58ae920c809caa23ced663ed10223de))
- **back:** отключена проверка авторизация для бэкенда пока не готов фронт ([de6abc1](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/de6abc17aafd03b6a7946bdead13ae8a1d72d8e9))
- **back:** регистрация представлений на бэкенде по имени класса сущности ([ff92db7](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/ff92db7e701ef2b89b6ae473cf6f61488e583aa7))
- **deploy:** добавил докерфайл во фронт  ([8efbd33](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/8efbd33ead3c88c67f1dae93373762e82c3ea7da)), [#688](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/688)
- **deploy:** не добавил докерфайл ([f726d65](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/f726d65db7108772573565f925165386bab36fe6)), [#688](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/688)
- **deploy:** перенос публикации в платформу ([57369c8](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/57369c8037a72ca66a8a51f3e5e25aa44693e56d))
- **deploy:** поменял докерфайл  ([d4757b1](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/d4757b15b3edf6c705c366f7d65704e1a82b458b)), [#688](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/688)
- **deploy:** правка пути ([442536a](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/442536a544e5d464659273a9242795ef4141d71c)), [#688](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/688)
- **deploy:** правка сборки ([6daa7dd](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/6daa7dd8e4ee53307142ecd7a6afa6f2b6a7e5f4)), [#688](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/688)
- **deploy:** фронт слушает порт 8080  ([ea7739b](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/ea7739b5fc9b832ce785928d6fc4449bb7fc17e7)), [#688](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/688)
- **dockerfile:** исправлены замечания sonar в части dockerfile ([fc8cd1f](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/fc8cd1f9b67a162d6fea032b77e8c8cc33175644)), [#287](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/287)
- **dockerfile:** исправлены замечания sonar в части dockerfile ([8bc6eb6](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/8bc6eb6e742927e0a80f117f1e2b0cd5c7a0f622)), [#287](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/287)
- **documents:** исправлено вычисление индекса готовности лица в ЖК ([6a4db39](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/6a4db39e6a0cb86e6b17f745b5651bdd0a539ed1))
- **entity,back:** генерик метод удаления списка объектов в репозитории ([10dcfc8](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/10dcfc8751797a2242ba91144f0b393c42e12adb)), [#726](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/726)
- **flexberry:** правка ошибки ограничения по полю мастера ([38a7da0](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/38a7da00cee587a9cdc10199fca44498ab486671)), [#569](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/569)
- **react,lookup:** изменен размер модального окна в лукапе ([9f032ff](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/9f032ff03904eebedcd359930657f4975092d2d2)), [#838](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/838)
- **react:** восстановлена работа токенов темы ([7d16090](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/7d160903c8ba161afe2e522b7e40b3e330c45b2b))
- **react:** доработка логики выделения строк списка ([3e71135](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/3e71135eb4756c702cae9b6ea0def67cc3d89a44)), [#720](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/720)
- **react:** изменены стили компонента button ([26fd608](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/26fd6085ce032d333d6ab8619bcc7824b83d1be1)), [#655](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/655)
- **react:** исправить ошибку с переходом на форму смен после "мои задачи"   ([a69d8eb](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/a69d8ebf32d08cc1b5dc92540440f65c7f12e64b)), [#806](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/806)
- **react:** исправлена ошибка импорта компонента ([cb1df69](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/cb1df69dc38ca14d60c1c1d29d07827875217416))
- **react:** исправление запуска мобильного приложения ([8c374de](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/8c374debbc0a34642a10e731f7d36a264377ba5f)), [#678](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/678)
- **react:** исправление ошибок нативной формы редактирования и лукапа ([67f4e2d](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/67f4e2dbaf1ab176481ae5b67230057ecdaee266)), [#823](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/823)
- **react:** обновление компонента TmobisButton с v3 до v5 ([694f716](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/694f71601cd93039e74bd948e81b2d348b49112e))
- **react:** обновление токенов темы ([3159798](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/3159798a80b662bde222133e1cb439b29d779841))
- **react:** перенос роута с таймером в правильную папку ([d332f11](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/d332f11cc774057fc4d075c03be87c20162b00fd))
- **router:** исправлено возвращение назад к списку после создания новой записи  ([13f7962](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/13f79621a84a44575442197a93c23a3ceae04afc)), [#801](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/801)
- **sign:** исправить подписание ол членом комиссии  ([c415ee5](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/c415ee582ef30423a8fc675d9a2f3b21e5d71ecd)), [#251](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/251)
- **yarn:** убраны дубли пакетов в конфиге Yarn ([23c7585](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/23c7585373384e989dab9fee755a12431baff80c)), [#796](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/796)

### Документация

- добавлена инструкция для агентов ([d8be762](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/d8be762cd6067390049857a3e51f971a2b39471e))
- документация Платформы ([b80e088](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/b80e0881cbb513373133439e130cf052b2a36258))
- документация Платформы ([db6a1df](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/db6a1df4e88bad93a6c3f66301bb0cfce7e548ec))
- **back,entities:** описание работы с сущностями на бэкенде ([3f79a32](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/3f79a326e24c68fcf19f3073699f176fe34d220b)), [#188](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/188)
- **react:** инструкция по генерации моделей на фронтенде ([77764fe](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/77764fedd2d09ee516b67f594d1688c4f3d05327))
- **react:** начало документации по React - статья по роутингу ([b78de19](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/b78de19c1a7dcdc00cc4240134034c4d728de33f))

### Прочее

- **react:** убран неактуальный комментарий ([ceb1174](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/ceb11745ddaf418469ee30ad98bcde5fd9a1f024))

### Рефакторинг

- удален старый код на Ember, NextJS и Flex WebAPI для NextJS ([651ad36](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/651ad36c0ac3643edef05f1ed78ca7ccc823c3ec))
- **react:** изменение контекста entity-edit ([97c0178](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/97c0178e2081ed75acb6bfe12e999bd4ef458aab))

## [4.3.7](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.3.7) (2026-04-28)

### Изменения

- работа с репозиториями в режиме транзакции + бизнес обработчики ([aca4c85](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/aca4c854f3d7f20eb24feba0d803eacda2788bd5)), [#598](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/598)
- Redis adapter ([a5dd048](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/a5dd048953d74755e2589807eb3fa3122b2086c6)), [#590](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/590)

### Исправления ошибок

- исправление определения генерик типа сообщения ([3d29572](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/3d29572715fa5ac86516d10fce7204c926eb2bc9))
- **dockerfile:** исправлены замечания sonar в части dockerfile ([d21fcc9](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/d21fcc9262b9e56650529084255534e55e09ca8a)), [#287](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/287)
- **reports:** поправлены пф ОЛ и программы ([d3cad03](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/d3cad03e9e1cdb87c3a38277b86c0d69559a9503))

### Рефакторинг

- исправлены замечания сонар по ошибкам и качеству кода ([8936e1b](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/8936e1be0c326d66d826994aff7352a656f0a44a)), [#597](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/597)

## [4.3.5](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.3.5) (2026-04-21)

### Исправления ошибок

- **documents:** изменён формат даты в пф огоп ([42dcc3a](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/42dcc3a6a4e036da416f214a0053f55b417c0847)), [timepath/platform/tmobis/products/ddtc#102](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/102)
- **documents:** изменена ПФ программы, добавлено ограничение на вычитку акта ООГ по ОЗП ([0f98378](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/0f98378a9198c57eeaf1a9448201b17040f76c2d))
- **documents:** правка весов в ол ([6372de4](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/6372de4728bd6c4c65c01772dbf980ccd09b0489)), [timepath/platform/tmobis/products/ddtc#66](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/66)

### Рефакторинг

- проверены потенциальные уязвимости регулярных выражений ([39ed575](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/39ed575cdac3e0b1c1928d0d6d9b56bacac3e922)), [#596](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/596)

## [4.3.4](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.3.4) (2026-04-16)

### Исправления ошибок

- исправление форматирования табов в PDF программы ([4e3094b](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/4e3094b8b2455a738ac900f37a8731926aec56ff)), [timepath/platform/tmobis/products/ddtc#102](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/102)
- поправлен формат индекса готовности и формат времени ([00ee9f2](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/00ee9f25b1186aa72943bef495f272b3c5e0badd)), [timepath/platform/tmobis/products/ddtc#93](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/93)
- **bi:** перенесена папка со скриптами BI ([e8a9797](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/e8a979786e15e8b362b3ea6123c93e938d90d70b))
- **documents:** вывод номера программы в акте оценки вместо номера постановления ([b98355b](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/b98355b952a1bfd4205176cfde5e9faf2b640150)), [timepath/platform/tmobis/products/ddtc#196](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/196)
- **documents:** изменен заголовок таблицы в ПФ ОЛ ([57d7735](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/57d77350d78d17aa5fe0db03f6cbb313c8a12f38))
- **documents:** исправлен расчет индексов оценочного листа для бесхозных объектов ([66eecfa](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/66eecfaf962a6cbbc1d061f42b56c95f203aa00a)), [timepath/platform/tmobis/products/ddtc#66](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/66)
- **documents:** ускорить процесс создания программы ([4f1c412](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/4f1c4122a939928f148dc6f976e5df1944003c6f)), [timepath/platform/tmobis/products/ddtc#223](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/223)
- **file:** изменение api/file -> api/fileload ([62a3245](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/62a3245ab71a59d59801c1dcd1db01f96e8243ec))

## [4.3.3](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.3.3) (2026-04-13)

### Изменения

- **ef,react:** реализация API React -\> EF ([664a6e3](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/664a6e30a6f0e892b3dfb81a63f9a7cd8834d344)), [#591](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/591)

### Исправления ошибок

- **documents:** правки реализации ролевой модели ([0e1fdb1](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/0e1fdb1b20e60da1be214e5b447795267fb35a23))
- **documents:** фикс странного поведения возвращения результата, всегда приходило false ([3ef3e93](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/3ef3e93924d1861dda7ddb026b6e2448d49273e9))
- **report:** изменен индекс готовности, логика показа страницы 2 ОЛ ([db772d9](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/db772d96b7a68b84b43dd9671cdd8e2f350deb51)), [timepath/platform/tmobis/products/ddtc#93](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/93)

## [4.3.2](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.3.2) (2026-04-09)

### Исправления ошибок

- **documents:** исправление удаления подписи после отмены подписания, доработка вычисления комиссии ([a1c7b97](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/a1c7b97366f1e3cfacc485c09707dc343ca65826))

## [4.3.1](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.3.1) (2026-04-08)

### Изменения

- **flexberry:** переопределен пакет логирования NewPlatform.Flexberry.LogService ([137d034](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/137d034b213d8eda5b0e8d3b680ff10f0eddedd6)), [timepath/platform/tmobis/tmobis.core#558](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/558)

### Исправления ошибок

- **document:** исправление логики прикрепления актов в ЖК ([c1ad03e](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/c1ad03ec0a1565fe68e8759f33210aef291196f9))
- **documents:** ограничение на формат файла ОЛ, исправление недочетов по актам ([796db8d](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/796db8d78a976fda309586aa97a688461a159efb))
- **usersession:** исправление ошибки с UserSession->User ([2b3116e](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/2b3116ec66cf230288eb5644fb297ffdb00cf6b2))

## [4.2.18](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.2.18) (2026-04-03)

### Изменения

- **admission-committee:** выдача роли "Член комиссии" при утверждении комиссии ([323141c](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/323141ccaa96b90b52f51365407073f05c26c9f7)), [timepath/platform/tmobis/products/ddtc#184](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/184)
- **awpassessingprogram:** добавить печатную форму программы оценки готовности к ОЗП и кнопку скачивания ([2fff2aa](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/2fff2aa905ae339cc1d69c943e4533fbb6115384)), [timepath/platform/tmobis/products/ddtc#102](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/102)
- **logging:** разработать адаптер для Serilog ([1185a0c](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/1185a0c63b1ac02ad568f1492788bd06d059004a)), [#568](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/568)

### Исправления ошибок

- **evaluationsheet:** отключить построчную проверку комиссией ([8b517d1](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/8b517d135d30881c0370641736e0f4628ad3610f)), [timepath/platform/tmobis/products/ddtc#178](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/178)
- **jwt:** абстрагирована работа пакетов парольной аутентификации и JWT авторизации для EF ([ba9681b](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/ba9681b615d9bcaf234d22a78ca57294bcbd9e95)), [#585](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/585)

## [4.2.17](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.2.17) (2026-04-03)

### Изменения

- **core:** улучшить регистрацию producer ([757770c](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/757770c0241756694713577820b4a41c9241774f)), [#564](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/564)
- **usersession:** абстрагирован класс UserSession от Flexberry ([a3bfcf2](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/a3bfcf29d63c4244eecb5d6e658d922cb20ff954)), [#578](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/578)

### Исправления ошибок

- **calculation:** исправления по расчетам ([6c933e0](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/6c933e0ef70de316d32534122734b0bc36abce51))
- **document:** бага с ограничением по статусу ([d6d1304](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/d6d1304537148263f148d07a11bf5163dd57729f))
- **documents:** доработка подписания в документах ([0ecdb01](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/0ecdb01a36408171822c2ba1701d684dea207b49)), [timepath/platform/tmobis/products/ddtc#192](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/192)
- **report:** доработана ПФ ОЛ согласно правкам ([6079172](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/60791729ae2a92afd4f7225076f76e427fb0fee6))
- **security:** адаптированы методы сложности пароля ([5c0e5be](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/5c0e5beaa239199c2231e82ed8773ab78609e7ce)), [#587](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/587)
- **uploaderFile:** проверена работа сервисов после абстрагирования FileUploader.Local ([0872d51](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/0872d51bf97fd8dc69bc27f5628a2ce06293fc15)), [#588](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/588)

## [4.2.15](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.2.15) (2026-04-01)

### Изменения

- **react:** тест роутинга в Expo ([8421985](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/842198562500cb8e69e2847a8bc0fe8ab23acaf7)), [#592](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/592)

### Исправления ошибок

- **admission-committee:** очищать зампреда при удалении сотрудника из детейла ([fc5820a](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/fc5820ae8bbbe92f363c60ab1ff135c38526dfcd)), [timepath/platform/tmobis/products/ddtc#87](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/87)
- **document:** исправление логики добавления подписантов в акт ([8d1ea2a](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/8d1ea2a1791f9e0c5eb762f485ea36e706e3a792)), [timepath/platform/tmobis/products/ddtc#80](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/80)
- **lookup:** доработка лукапа ([67dd4fe](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/67dd4fea961405e86fcb50ec8a836e198ce5fcc7)), [timepath/platform/tmobis/products/ddtc#23](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/23)

## [4.2.13](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.2.13) (2026-03-30)

### Изменения

- **document:** реализовать функционал автоматического расчета уровня готовности в ООГ и ПГ ([b4513ec](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/b4513ecb8d8de2cd265337977cd779f764b9844d)), [timepath/platform/tmobis/products/ddtc#119](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/119)
- **documents:** автоматическое добавление подписантов в акт ([aef8544](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/aef8544ae0b75fd2c27abdef879fd3935f788d4a)), [timepath/platform/tmobis/products/ddtc#80](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/80)
- **react:** добавлен сэмпл на Expo ([c5e1150](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/c5e1150e14857accbb3fab65cac44749fb46a330)), [#574](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/574)

### Исправления ошибок

- **document:** доработка логики журнала контроля ([3474325](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/3474325f9be2f21a0e3538c500e3aa93a679a4c8))

## [4.2.12](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.2.12) (2026-03-26)

### Изменения

- **metrics:** пакеты для сборка и вывода метрик в Prometheus ([47d79a9](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/47d79a9eebe19ca1451375eef517c2e8c78d5f9d)), [timepath/work-process/work-process#9](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/9)

### Исправления ошибок

- добавлен потерянный проект ([521c01d](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/521c01d4046f89048ff8e7209bdf1e5094f8fdf7))
- **commissions:** исправление замечаний по комиссии ([41ee25a](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/41ee25ae48e6f3284264d11e07607226d374abc8))
- **documents:** формирование объектов программы из иерархии объектов ([b370a3a](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/b370a3a59dfacc26fb2a8db6606a1d5bccc54749)), [timepath/platform/tmobis/products/ddtc#170](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/170)
- **uploadFile:** исключено использование Flexberry в Tmobis.Files.Uploader.Local ([8ccf86d](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/8ccf86dffa73d1edb3c31dcb58b0e93740a3234e)), [#562](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/562)

## [4.2.11](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.2.11) (2026-03-25)

### Изменения

- **calculations:** расчеты по оценочному листу ([6c2897f](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/6c2897f8bef62d78fef1c6cdb14104d70ec9a14a))
- **ef:** поиск интерфейсных агрегаций для модели EF ([fe03d8d](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/fe03d8dd1f57a59ddf3488118987e24ad7022ac0)), [#566](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/566)
- **email:** реализована логика отправки Email ([c6f6715](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/c6f671596b8a20334cbd1ce0b480853dd2f0f3d7)), [#408](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/408)
- **repository:** представления для выгрузки данных из Repository + реализация методов EFRepository ([596042c](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/596042c9b1c062c011eb4f2e21cb0791d6bfc0e0)), [#565](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/565)
- **security:** Перенесы методы сложности пароля из МИМК ([a467141](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/a467141b1377bfc9ae352a4f244437378c149210)), [#573](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/573)

### Исправления ошибок

- **document:** настройка ролевой модели ([1794b8d](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/1794b8de0867e9ce7fe3290688e0155f2e657625)), [timepath/platform/tmobis/products/ddtc#25](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/25)
- **ef:** временное создание scope при создании efrepositoryproxy ([64d0628](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/64d0628d34b424097780542311dc3f8e5f5ae28f))
- **employee:** исправление формы Сотрудника ([622b8c7](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/622b8c731b60245d527e1f1e0006c5ee85eda3f0)), [timepath/platform/tmobis/products/ddtc#154](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/154)

## [4.2.10](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.2.10) (2026-03-23)

### Изменения

- **document:** реализована логика формирования журнала контроля ([0b030b5](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/0b030b5fd04371a6ce26019725ec3617f147a5d6)), [timepath/platform/tmobis/products/ddtc#136](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/136)
- **listener:** улучшена регистрация слушателей ([15b0846](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/15b0846272099d1b0eae782aff1a1149097b61d9)), [#563](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/563)

## [4.2.9](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.2.9) (2026-03-20)

### Изменения

- **sign:** исправление замечания по подписям ([605bcbe](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/605bcbe4e5e408143f228455731dfb21b4eb08c3))

## [4.2.8](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.2.8) (2026-03-20)

### Изменения

- Добавлен заместитель председателя комиссии ([9ae1f7a](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/9ae1f7ac86ccdf8cef93d0a0b46f57625b3621ed))
- **document:** добавление фильтров на список журнала контроля ([f0986c1](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/f0986c12a107dac0534c47fddba27f5662663782)), [timepath/platform/tmobis/products/ddtc#100](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/100)
- **document:** добавлены списковая форма и карточка журнала контроля ([a108b69](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/a108b69a2104ffc6adebe6d23307c6ec86e34bc7)), [timepath/platform/tmobis/products/ddtc#100](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/100)
- **documents:** реализовано ограничение на сохранение ООГ ([89178d6](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/89178d6f0a9e0d9c9195f004d7b79350d5d1d0dd)), [timepath/platform/tmobis/products/ddtc#123](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/123)
- **ui:** скрыта кнопка настройки отображения столбцов на списковых формах ([c2c8950](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/c2c8950660f63cd6f05a95a863b4e38e9e404b37))

### Исправления ошибок

- **commissions:** исправление замечаний по комиссии ([b77f71e](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/b77f71e685b1c0091ac5d00d5de8d746e1873601))
- **document:** испрвлена ПФ акта оценки и добавлен вызов ПФ оценочного листа ([895944c](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/895944ca89a2343e12dcb368b9d5796f1e6fffe3))
- **employee:** исправлена проверка на дубли пользователя у сотрудника ([2fe15ec](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/2fe15ec39ecb70cfb7ad8d1ed2e3e74efdc8bc15)), [timepath/platform/tmobis/products/ddtc#98](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/98)
- **organization:** исправлено имя предиката лукапа муниципального образования  ([f39d0d9](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/f39d0d92f454717f0f6d7d43b221fb79d9b9e3ba))

## [4.2.7](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.2.7) (2026-03-17)

### Изменения

- **batch:** добавлено сохранение тела batch запроса при ошибке ([99816e4](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/99816e4278b796047ea6a8db50636996e2b51c9e)), [#456](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/456)
- **documents:** добавлены подписи в документы ([46ce9c7](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/46ce9c79d3f3e2249540f622e31eddc5ab69fda5)), [timepath/platform/tmobis/products/ddtc#80](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/80)
- **ef:** реализована возможность вызова списка IDbContextConfigurator при создании контекста ([c3f0a43](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/c3f0a43a3c40d6c88dd399b7c23f08545ffa760c)), [#567](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/567)
- **nextjs:** классы для сэмпла на Nextjs ([ee881ba](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/ee881badd8aa3f9814150561b7103d22b96835bc)), [#555](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/555)

### Исправления ошибок

- **document:** правка удаления акта со списка ([50a6a15](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/50a6a1538455f1e7c96182454172fb8e1132108b)), [timepath/platform/tmobis/products/ddtc#40](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/40)
- **document:** правки по ОЛ, исправление формирования номера документов ([8281a06](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/8281a063978138fbd4959d9f80453c9fc214f69b)), [timepath/platform/tmobis/products/ddtc#26](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/26)
- **employee:** исправление сохранения Сотрудника ([73d27ca](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/73d27cab414f8dc8e4a3887609dd20945ea1421b)), [timepath/platform/tmobis/products/ddtc#129](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/129)
- **employee:** исправлено назначение нескольких сотрудников одному пользователю ([a48b1b1](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/a48b1b146492e7a1abeada224900a30ade78f643)), [timepath/platform/tmobis/products/ddtc#98](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/98)

## [4.2.6](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.2.6) (2026-03-12)

### Изменения

- добавлено возвращение ответа об отправке писем с бэкенда на фронтенд ([e299b05](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/e299b059cd1ee52d1bae3c9da23a2b192f1939ee)), [#330](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/330)
- **business:** лукап пользователя на форме сотрудника ([93664c5](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/93664c50971988c503d99824cbb0f3624e1948ad)), [timepath/platform/tmobis/products/ddtc#98](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/98)
- **email:** добавлена обработка запросов на отправку Email с ПФ ([04edf31](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/04edf31f230227b646052ef06b77b8776830ea27)), [#406](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/406)

## [4.2.5](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.2.5) (2026-03-10)

### Исправления ошибок

- **front:** исправление удаления со списка ([2a163c3](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/2a163c354b195b4828b4fce7c718b78e24f1321a)), [timepath/platform/tmobis/products/ddtc#111](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/111)

## [4.2.4](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.2.4) (2026-03-10)

### Изменения

- изменен сервис app-header ([90fb2f0](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/90fb2f00c4e44e7698b3c4d25ef0d4c462c391d7)), [timepath/platform/tmobis/products/ddtc#121](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/121)
- **nextjs:** новые компоненты Nextjs от технологов ([0c76f60](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/0c76f60d7c4747137871f04d8134becacb257f49)), [#555](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/555)

### Исправления ошибок

- **business:** исправление замечаний на форме ПОГ ([60ca4f6](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/60ca4f6ad130b202184ef01aa6362aed227e0cc5)), [timepath/platform/tmobis/products/ddtc#52](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/52)
- **front:** удаление битого комментария в роуте admission-committees ([1563788](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/156378823556c35f589fc3bb5c47f86c4c43004d))

## [4.2.3](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.2.3) (2026-03-06)

### Исправления ошибок

- **front:** правка багов после тестирования стенда ([22f2442](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/22f24428ef474758a02a357796ed163c559b0718)), [timepath/platform/tmobis/products/ddtc#23](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/23) [timepath/platform/tmobis/products/ddtc#28](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/28)

## [4.2.1](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.2.1) (2026-03-06)

### Исправления ошибок

- **business:** добавлены IRegisteredEntity потерянным бизнес-сущностям ([7f1a91b](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/7f1a91b5d8fb710156d68da238205541a171b697)), [#409](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/409)

## [4.2.0](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.2.0) (2026-03-06)

### ⚠ КРИТИЧЕСКИЕ ИЗМЕНЕНИЯ

* сущности материализуются только при реализации нового интерфейса IRegisteredEntity
* IAuditableEntity превращен в IAuditable и встроен в IEntity

### Изменения

- **ef:** базовый модуль работы с EF ([c07eac3](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/c07eac36068beb582e190ae27ae1f06bd06e99eb)), [#409](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/409)

### Рефакторинг

- изменены стили компонента flexberry-objectlistview ([0c6ce60](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/0c6ce60eda5568f8e8216fd48eb020afec9e20d1)), [timepath/platform/tmobis/products/ddtc#90](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/90)
- **ui,ember:** компонент flexberry-checkbox изменен в соответствии с темой ([373e414](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/373e41484a86b7158187f2383aaf9118e95ca11a)), [timepath/platform/tmobis/products/ddtc#89](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/89)

## [4.1.17](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.1.17) (2026-03-05)

### Исправления ошибок

- **document:** исправления в ОЛ ([97c3a1c](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/97c3a1cbfc8df44b5eed78245c556f72efb9e69c)), [timepath/platform/tmobis/products/ddtc#26](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/26)

## [4.1.16](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.1.16) (2026-03-03)

### Изменения

- **back:** переписаны FlexberryOData Handler'ы ([06372f8](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/06372f851800042d8200ee30f4ceebdf7c311549)), [#466](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/466)
- **document:** построчная проверка детейлов на форме ОЛ ([2e7842c](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/2e7842c35c9bf7b8e0df7e1f92598e25e368b46e)), [timepath/platform/tmobis/products/ddtc#76](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/76)

## [4.1.15](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.1.15) (2026-02-27)

### Исправления ошибок

- **documents:** исправление опечатки в валидаторе детейла программы ([e51ffdb](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/e51ffdb2b8431a8fe64970bac43d1faa61bbec2d)), [timepath/platform/tmobis/products/ddtc#34](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/34)

## [4.1.14](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.1.14) (2026-02-26)

### Изменения

- **calculation:** расчеты по ол ([5a415ee](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/5a415ee57675883522f98f30aee19e32043ca764)), [timepath/platform/tmobis/products/ddtc#68](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/68) [timepath/platform/tmobis/products/ddtc#70](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/70) [timepath/platform/tmobis/products/ddtc#66](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/66)
- **ember:** раскрытие тогглера по умолчанию по флагу из конфига ([b7d734f](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/b7d734f6ab896ee0707005d68d21120646130834))

### Исправления ошибок

- **documents:** исправление замечаний в программе ([647f9df](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/647f9df31bf3becc8e1389c5ac655b5e958bb60c)), [timepath/platform/tmobis/products/ddtc#34](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/34)
- **documents:** пакет ОЛ добавлен в солюшен ([930af14](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/930af141bd16b1aaee7d4fe01181b4beae9ca0e0)), [timepath/platform/tmobis/products/ddtc#26](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/26)

## [4.1.13](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.1.13) (2026-02-25)

### Изменения

- **documents:** акт оценочный лист ([18e2b29](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/18e2b29a873d83d7e7aa07be33643d74c02a8b0d)), [timepath/platform/tmobis/products/ddtc#26](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/26)

### Исправления ошибок

- исправление работы multi-text-lookup ([f3524e3](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/f3524e35179987c501d5847f333c5480c565f795)), [timepath/tmobis.mk/tmobis-mk#1288](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/1288)
- **document:** доработка акта оценки готовности ([d835a52](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/d835a525e8c05d2f8ebb0f95db93eda83e4e4089))

## [4.1.12](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.1.12) (2026-02-24)

### Изменения

- **document:** в акте оценки добавлена кнопка скачивания ПФ ([f811039](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/f8110397b7aa81d00e415a523fc6db2813a49ab0)), [timepath/platform/tmobis/products/ddtc#64](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/64)

### Исправления ошибок

- **layout:** изменена верстка форм ([3f3cced](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/3f3cced731429521cdf1d72b1243fe8aac024bbf)), [timepath/platform/tmobis/products/ddtc#62](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/62)

## [4.1.11](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.1.11) (2026-02-19)

### Изменения

- **document:** доработка акта оценки ([5cf11cb](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/5cf11cb5b27d59b8f82895729b103b5792b348df)), [timepath/platform/tmobis/products/ddtc#40](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/40)
- **reports:** кнопка скачивания PDF печатной формы документа ([3ab03c7](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/3ab03c77a9143a918d1e068660585fa334fda660))

### Исправления ошибок

- **front:** исправление ошибок по справочникам ([0b0d801](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/0b0d801a0b84ccf2a674aef806710394748ada37))
- **log:** изменение регистрации syslog через ILoggingBuilder ([42b55ee](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/42b55eecc4fb6586491222dd07f8049d479a6a91))
- **package:** правка бага при сохранении формы ([6ba5903](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/6ba5903644fd594387ebd0cfcaef4b5d6e1dba5e))

## [4.1.10](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.1.10) (2026-02-17)

### Изменения

- **documents:** печатная форма паспорта готовности ([a4fce00](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/a4fce0006511c0eeb78f041d69d5f1b450169535)), [timepath/platform/tmobis/products/ddtc#60](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/60)
- **template:** сборка и развертывание шаблонных стендов ([94c408f](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/94c408fe3258db635995582a87e167deb6fbb195)), [#485](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/485)

### Исправления ошибок

- **deploy:** изменение deploy для развертывание стендов ([9a9d16b](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/9a9d16baa7de88b23524aa753b4fd9f56f36586a))
- **documents:** перенос паспорта в отдельный пакет ([f8e958b](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/f8e958bda88ca62e7e270621d3deec8c5919952d)), [#52](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/52)
- **template:** перенос папки ansible deploy в корень для деплоя стенда платформы ([2bf03f1](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/2bf03f135c1dd4aa40aee0ccafa6634c4a031665))

## [4.1.9](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.1.9) (2026-02-16)

### Изменения

- **cordova:** перенос настроек для тестовой сборки АПК в Платформу ([d3e13fb](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/d3e13fb57116b6d4b767b404be2a22f9828b56ed)), [#551](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/551)
- **documents:** паспорт обеспечения готовности ([d6390db](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/d6390db9405833a87bb62db46e5633d1ac06a911)), [timepath/platform/tmobis/products/ddtc#52](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/52)

## [4.1.8](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.1.8) (2026-02-15)

### Изменения

- **documents:** паспорт обеспечения готовности ([5bafd21](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/5bafd210d832c0f42acadedbd1f84c455735ea42)), [timepath/platform/tmobis/products/ddtc#52](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/52)

### Исправления ошибок

- **ef:** исправлен путь к ядру в пакете EF ([7b70c70](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/7b70c70dbf01babc3a813f3300d5bc3dff87c668))

## [4.1.7](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.1.7) (2026-02-14)

### Исправления ошибок

- **documents:** исправление ошибки с ключом типа программы оценки ([44e2739](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/44e2739ce77e8adfe1c7e6dbfaef3066348c98ee))

## [4.1.6](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.1.6) (2026-02-13)

### Изменения

- **business:** добавление мест работы у employee ([5e1f697](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/5e1f69795fdf45299aedc4301873e5876ea59421)), [timepath/platform/tmobis/products/ddtc#28](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/28)
- **documents:** реализована ПФ Акта ООГ ([77cfd1b](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/77cfd1bf25ca2a2a24f09aed03eb68126439e6f2)), [timepath/platform/tmobis/products/ddtc#64](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/64)

### Исправления ошибок

- **ember:** возвращен порядок линковки аддонов относительно flexberry ([ad90e8e](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/ad90e8e8d3e8db8cd201a7a04eb47db4de3bb556))

## [4.1.5](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.1.5) (2026-02-13)

### Изменения

- **data:** наполнить справочник тип контрагента ([b0c9a1e](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/b0c9a1ef4c819c2491b3666b6d16893b87241e67))
- **documents:** акт оценки обеспечения готовности к отопительному периоду ([6e5fa47](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/6e5fa477b1cb2118da65702da391f3e253aa7def)), [timepath/platform/tmobis/products/ddtc#40](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/40)
- **documents:** программа проведения оценки обеспечения готовности к ОП ([22412a5](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/22412a519d3cf5401862a13dc14b9e51e4ce7011)), [#34](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/34)
- **ef:** пустой проект под модуль EF ([0523722](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/0523722e9b823e19d61effb423cf7bf3096baccf))
- **nextjs:** создан пакет ядра Платформы для Next.js ([858159d](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/858159d30cf2fdfdb6e6cd7a2cdfba80bef6aab7))
- **package:** реализовать добавление мест работы у employee ([649d30c](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/649d30c531f2e5da2425204d759ddbfd3ec7b2dd)), [timepath/platform/tmobis/products/ddtc#28](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/28)
- **stimulsoft:** обобщение метогда LoadReportData по строке подключения ([00a8836](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/00a8836d6d22177c26be15fc430564427ce0b11d)), [timepath/platform/tmobis/tmobis.core#471](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/471)

### Исправления ошибок

- **log:** проект tmobis.logging.syslog включен в решение Платформы ([8fa5b05](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/8fa5b05d8a0cc819a79830d18b0b9b100db77a9c))

## [4.1.4](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.1.4) (2026-02-07)

### Изменения

- **documents:** рефакторинг актов и программа проведения оценки ([4939b9b](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/4939b9b82aed5e74e4ed57ba43b5a251002c5a67)), [timepath/platform/tmobis/products/ddtc#34](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/34)
- **nextjs:** сэмпл приложения на Nextjs и Flexberry WebAPI ([d832a5e](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/d832a5e2e9b615bf55590bb0f46aff5be6dc8c68)), [#542](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/542)

## [4.1.3](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.1.3) (2026-02-06)

## [4.1.2](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.1.2) (2026-02-06)

### Изменения

- **back:** приборка в адаптерах работы с файлами на бэкенде ([56ef752](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/56ef7524e266913558e09cbed8cd68ea80063272)), [timepath/platform/tmobis/tmobis.core#480](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/480)

### Исправления ошибок

- **bug:** исправление ошибок по справочникам ([ca0efe3](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/ca0efe33d8b41b84343b9a6ffec649ef562aa7e4))

## [4.1.1](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.1.1) (2026-02-06)

### Изменения

- **backend,frontend,sample:** перевод sln в slnx, актуализация проектов и добавление шаблонного приложения ([8b08b4e](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/8b08b4edfd46e1a8adb7a19430772258738c0646)), [#484](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/484)
- **business:** дополнен справочник "Тип контрагента" ([ad4a802](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/ad4a802c2a66b01847f78aef2fccb87f37e8e8b6)), [timepath/platform/tmobis/products/ddtc#38](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/38)
- **business:** реализовать создание комиссии ([be066a6](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/be066a6fdaa197f7dacd5e05268bb733b783b0ed))
- **flexberry,sql:** добавлен скрипт создания функций для создания полномочий на классы по ролям ([f53cd7e](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/f53cd7e6bbba1958cec0d4d35d7bced4da33bf74))
- **log:** добавление логирования на бэкенде и фронтенде ([f623fd7](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/f623fd7117187370c15da157c0904dfbd8d652d0)), [timepath/platform/tmobis/tmobis.core#32](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/32) [timepath/platform/tmobis/tmobis.core#33](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/33)

### Исправления ошибок

- **sql:** исправление SQL скриптов для Liquibase ([8dc13f7](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/8dc13f79ca68505d102c9ce030787f587d985e0f))
- **sql:** исправление SQL скриптов для Liquibase ([5691fad](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/5691fad078b1a85d5c835820a909ec7298e498c9))
- **sql:** исправление SQL скриптов для Liquibase ([b68798b](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/b68798bd02f0a73bdb7235301127e138272b5e7f))

### Прочее

- **template,sql:** чистка лишних скриптов в шаблоне OData ([e87334f](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/e87334fe35574e7ec8a3f4cdbfea4368de9cdde7))
- **template,sql:** чистка лишних скриптов в шаблоне OData ([21e5284](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/21e52847a55609ad4f00dc987e02d8bf3a53ba0d))

## [4.1.0](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.1.0) (2026-02-01)

### ⚠ КРИТИЧЕСКИЕ ИЗМЕНЕНИЯ

* **back:** обновление библиотек и SDK до версии .Net 10

### Изменения

- **back:** обновление до .Net 10 ([8d3611d](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/8d3611dd69b7398a6b5234300960d0ad69b645f5)), [#483](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/483)

## [4.0.2](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.0.2) (2026-02-01)

### Изменения

- **back,windows:** добавлен WindowsServiceModule для запуска сервисов как служб Windows ([d17c6ef](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/d17c6ef4df1d96bf9f9adfc0c75fb7569da21307)), [#541](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/541)

### Прочее

- **front:** чистка корня репозитория от лишних файлов и пакетов в package.json ([0b8147a](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/0b8147a877216026ac7f6b44938b48e1b936b028))

## [4.0.1](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.0.1) (2026-01-30)

### Изменения

- **changelog:** добавлены commitizen/lint и conventional-changelog для генерации чейнджлога ([380f01a](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/380f01a70ff8184c6af90e57f423594f13766b6c))
- **core,backend:** добавление авторегистрации консумеров ([e3b32c8](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/e3b32c83d8f44a402dcc02a48144c9a965c2fd82))
- **core,backend:** интерфейсы маппера ([0774ad8](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/0774ad855bcc772d703c2a2ff6936976f3e8f175))
- **documents:** добавление base-act, нумерация документов и удаление ненужных файлов ([7838a77](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/7838a779e9e1205e93a27e85080ccc9ad2194cea))
- **documents:** добавление подписей в платформу ([da48543](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/da4854314825373a99c0f41baa8a1a002a0db119))

### Исправления ошибок

- **business:** правка замечаний по справочникам КА, типа акта и назначения объектов ([752ecdb](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/752ecdb6d8f6b1d0d3e21e7f881e65822e7212cd))
- **core,frontend:** проброс this в метод markAccess контроллера ([b538376](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/b538376645107cbeb0fa7084ce422d87705d1275))

### Производительность

- **offline,rabbitmq:** оптимизирована сборка оффлайн-бэкапов и исправлен RabbitMQ listener ([e0b9e16](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/e0b9e163d2511067182a6bb80f7d9d21737a82f9))

### Документация

- **changelog:** автогенерация чейнджлога с помощью conventional-changelog ([dd51f4e](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/dd51f4e0484dbcfdc017e0d6aa597ecffc230e11))
- **readme:** актуализация README ([7c16ca3](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/7c16ca3abba67a3cf298a5960ac66c347a481fb9))
- **readme:** добавлена информация о правилах проверки Conventional Commits ([987f979](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/987f979ae3b7c567e01b7428704316401e9b747b))

## [4.0.0](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v4.0.0) (2026-01-22)

### Изменения

- IRepository.MakeEntity сделан публичным ([3ccab08](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/3ccab085832fbe7c51b4a56b13dbb0f3a9b87c64))

### Исправления ошибок

- исправлена ошибка мерцания шапки ([40589ed](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/40589eddac21de064bd88ea3e08422a3cc25b9c2))
- Исправление лишних хэшеров паролей ([da86a32](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/da86a32bd7195947a6e25f06616297873c758e5d))
- селектор .ui.sidebar применяется и в desktop, и tablet окружениях ([a1ec595](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/a1ec59584cb025cfeddb90e2170477299ccba4d6))
- Убран неправильный способ регистрации SqlParameter в сервисе ПФ ([b20b6a4](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/b20b6a440163a08ad0a3193ad81181cf27362e31))
- Чистка мусора в корне репозитория ([721dc7d](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/721dc7db156033bf68c55d6a5a5f8846cad38c34))

## [0.8.62](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v0.8.62) (2025-12-30)

### Изменения

- Поднятие версии CoreWCF для устранения потенциальной уязвимости ([da55d48](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/da55d48089779caba2bea2159c7b4ea2544ba7b7))
- Поднятие версий уязвимых пакетов ([d15ab36](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/d15ab360857239e0e2a3252199f24aeb71782769))

### Исправления ошибок

- Исправление ошибки пути к проекту ([7f27f9a](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/7f27f9a6108f35b36fb86bc2dda2391e43caa746))

## [0.8.61](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v0.8.61) (2025-12-18)

### Изменения

- Добавить логирование при выгрузке отчета (signalR) ([d2528b9](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/d2528b9bb345cc5d76bcec4f350e8b0d5709731d))
- Добавлен сервис для сохранения позиции скролла и закрепления шапки ([bf71ac5](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/bf71ac5b1b75e59e1c9409e658ad87738021d26a))
- Добавление свойств в tmobis-photos ([4ff9574](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/4ff9574c7c6449b7bb4759d6dd04c392a051ac0a))
- добавлены новые стили для адаптивности ([108d1e1](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/108d1e19f984b4e3fb5539708a6c1d38ad90029f))

### Исправления ошибок

- Исправление URL выгрузки файлов офлайн ([cf3f772](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/cf3f772678c2d055e017edc1b640eeb78ff56e71))
- логирование при выгрузке как LogError ([6223428](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/622342892579243431cf71ede25fd174f416b3d4))
- Не сохраняются пользовательские настройки ([c8803c0](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/c8803c0eed6b0a681b1c991b3175f0e605006910))
- отключение свайпера при пролистывании фотографий ([57a62c2](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/57a62c22c8315e809ab900d0bd97c133296a488d))

## [0.8.56](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v0.8.56) (2025-11-11)

### Изменения

- возможность указать кастомный экшн для превью в tmobis-file-thumbnail ([d6047dc](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/d6047dc9f65e9f35f3775016572bb20df2ab36b5))
- Нормализовать работу tmobis-nav-tabs ([30c503f](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/30c503fef0ee76cad96c442b8b19be16c672b5d3))
- Оптимизация потребления памяти ReportService ([da77df5](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/da77df5c107382f3f14723cd8f76b3948012da78))
- параметр check/uncheck в событии 'выбрать все' в олв ([e09266a](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/e09266a5739d7f1984e31a1750106b41349ba0cf))
- Реализовать массовую выгрузку по пдф ([88f96c5](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/88f96c5e3e6a10e08f13693ff33ac084bc8546ed))
- computed proberties in groupedit added ([d37f9c5](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/d37f9c5a7fd95f63042709e4148af8c49171d397))

### Исправления ошибок

- вернула прошлые сообщения об ошибках ([e6cedd9](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/e6cedd92308925c0e9a7d0c51bf73df05791f39c))
- включение pointer-events в заблокированных полях ([659596f](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/659596f0884df6e750222199c1b0b5486a6310ba))
- воплощение пользователей ([008cb7f](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/008cb7fcc87fb802766387529cee67c72c7a96d3))
- восстановлена возможность копирования заголовка в шапке ([dcaa72d](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/dcaa72d9fe2254e547eed9beb74b091fdb38947c))
- Временный GC ([b31af9f](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/b31af9fa1b3f193cb93841db1702b9c053935fd0))
- Всегда проставлять дефолтный SecurityManager в DSFactory ([cab151d](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/cab151da961c844bcc702db3ec482f808bc22a4a))
- Диспоуз в сервисе ПФ ([7d66b16](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/7d66b163d1bc2772893b577763d8f842bbe72f6d))
- Дополнительные попытки очистки отчетов SQL ([603772e](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/603772ee568dd83058dc7750addc82ae30ee1f67))
- Еще попытка сделать Dispose отчетов ([57554c0](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/57554c08253865edd58dbd4881680d3886bccdc5))
- Замена ContentType на json в WebApiProducer ([0092946](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/009294639416449a27ab1e3300ed862572cf322b))
- изображения влазят в ширину модального окна ([8a810d5](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/8a810d516e10562eacb809379dec01fe92d294bd))
- Исправление массовой отправки почты и выгрузок ПДФ и больших отчетов ([871704b](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/871704b908f4dfbdaf41b0506083c691ed3b4772))
- исправление ошибки при возвращении на ПМЛ с акта ([3fe50e4](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/3fe50e41c02e961c5983f25d2992550bd9091e83))
- Многопоточное прослушивание Rabbit + ускорение массовых ПФ ([fa7716d](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/fa7716d6a00b347b0d2253bbf4bda29bc2c225f3))
- Не ожидать запуска фоновых Listener'ов ([1004bda](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/1004bdaa2e6fb6e0b73163ddfdb385ff35e38340))
- Не сохраняются пользовательские настройки ([bde3f66](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/bde3f66877305d3488856a7c66ad7e621fa5610f))
- Переопределение preview чтобы убрать readonly=true при переходе на страницу объекта из лукапа ([4e272ec](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/4e272ecc1956e5f61cb9457b23187eb971371c5b))
- Пересчёт отступов у дропдауна при onShow/onHide ([8fffb86](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/8fffb86fb6ca9cb1dadf9e7ec6002d5483e2083f))
- Пользовательские отчёты: кнопка редактирования не активна ([9d1fe2e](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/9d1fe2e985b229fb7eef5c57e61e7fa4a95e1be4))
- Правка для создания новых пользователей в системе ([c384072](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/c3840724463d912c934426cc9571de41ce6a3a0d))
- создание файла через CreateDataObject для проставления Id ([bfdd209](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/bfdd20952e3f858ecabf7295d301ccf7bc431269))
- сообщения о валидации в полях и на логин форме ([5b3a388](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/5b3a3887b82fedfd86328b672a66f3647db90f2d))
- убрала инлайн-стили модального окна ([501bd0c](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/501bd0c3600b3edee4d8298cc3dff4d3fa88f5e7))
- Убрана проверка на пустое значение параметра в SqlReportBuilder ([f5283cf](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/f5283cf3e43ba53386e5a615424af4f2d769cb8e))
- фикс overflow дропдауна ([21d9275](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/21d927518df4208ad5cf965c400141564c909085))
- Форсированная проставка CommandTimeout в DSFactory ([eb33af5](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/eb33af5784e3fc369e4c8ac9898cec4ce77f6413))

## [0.8.43](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v0.8.43) (2025-10-01)

### Изменения

- Апдейт в EmployeeProvider ([7abda92](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/7abda921ace5dc5a544c37aceb8228e388eb2af2))
- Зависимость модулей ([10714cd](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/10714cd7ed91cd28da3c14b5a2e987f107c9a505))
- использование File/Problem в ReportController вместо HttpResponseMessage ([57757b6](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/57757b6658e27e8f08fa0f520f4197f30387933e))
- Перенос InMemoryProducer в ядро ([43b2aa1](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/43b2aa1967e0921726abbf3a960e52113638cc3e))
- Старая отправка почты ([c92083e](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/c92083e02439baa5aec4e671eea99a0162b7208b))
- шапка для детейловых форм ([d84d2e0](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/d84d2e0109c8451dd92307725389141f1b1179fa))
- custom filters added ([852a85d](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/852a85d1eb44487ab9f33c5c207415b0c95af088))
- tmobis-photos — ранний return при пустом value (undefined/null) и модалка только при наличии fileIds ([03720bb](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/03720bb2895640dce5738aa0b73219cbd76aea60))

### Исправления ошибок

- Исправление защиты Excel + старые параметры SQL отчета ([2520656](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/2520656ffbf5ff6523d28c169cefbfa9663bdce8))
- Исправление ошибок раббита ([1d58d41](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/1d58d4118ac725ce6bd435afca937ce1865bf453))
- Исправление формирования отчетов SQL ([a76aa05](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/a76aa0519d2eb21ecf7c9d3e01da1b0e4cb15402))
- Отключен or isnull в userspecificlimiter ([d286cd9](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/d286cd916249e1732f25a16c78333c9e8d8f4474))
- отображение компонентов в строке мобильного групэдита ([57d6b5a](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/57d6b5a62557df88079ca4b2502be59be1394baa))
- Ошибка при смене пароля другому пользователю ([8a7339c](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/8a7339cad9992a82dc0caf95eb1f29b25ec8a497))
- Учет subtype при загрузке ПФ ([22c6656](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/22c665669191a22650a7acb28a60741dfec54b44))
- фикс высоты свайпера и локали в календарях ([8c315c3](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/8c315c3c953f6ba6262f79b3649c3a3d0f06ffa7))
- фикс импорта шрифтов + автоматическая смена активной вкладки в tmobis-nav-tabs ([df11c28](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/df11c28fd18c288398fa8b2312300caa58fe4d83))
- чекбоксы в групэдите, отображение компонентов без value в карточках ([57d8a46](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/57d8a46a2d8b7204c923b3da3e569ef7528566c1))

### Документация

- CHANGELOG по InMemoryMediatorModule ([5b8a33e](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/5b8a33e97fc438ecb71fba69da8162832cf9df9b))

## [0.8.36](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v0.8.36) (2025-09-03)

### Изменения

- **core/modals:** добавлен waitHidden(), hideLoading(true) стал ожидаемым ([67a5516](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/67a55166086303f16a20096f6bb6d00e9e3883f1))

### Исправления ошибок

- в willTransition добавила this._super(...arguments); ([0848620](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/08486209940899e1f208d8b74047d4689e18c901))

## [0.8.35](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v0.8.35) (2025-08-29)

### Изменения

- Настройка в конфиге для отображения наименования в шапке ([d895597](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/d8955977c74e7063f26b396219269f11073f27d1))

## [0.8.33](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v0.8.33) (2025-08-22)

### Изменения

- Выгрузка в онлайн ([df93d37](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/df93d373ca16c67f39846fabfdda9f7816a1682b))
- WebApiProducer ([0d75878](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/0d75878e6ff41cbfc88838965a7269bb3ec38c37))

## [0.8.32](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v0.8.32) (2025-08-11)

### Изменения

- Фоновый режим и повторные попытки перехода в офлайн ([40f7c68](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/40f7c68041659bc56e3ba77750e559d4f54a11ca))

### Исправления ошибок

- Приборка стилей и шаблонов инпутов ([15dea47](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/15dea4767ee3b56aa7984b6d841162792e7cc83f))

## [0.8.30](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v0.8.30) (2025-08-01)

### Изменения

- Переключатель перехода в офлайн ([b58a318](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/b58a3180d453f40eb72da8a1c033bbeaa323ebdf))

## [0.8.29](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v0.8.29) (2025-07-30)

### Изменения

- Переход в офлайн ([fe64dca](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/fe64dcaca5b2bee0bda5de1c7446510f6ea385b1))

## [0.8.28](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v0.8.28) (2025-07-23)

### Исправления ошибок

- Исправление ошибки в офлайне ([9885d2d](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/9885d2d2e62bc0f6d243f271a94de84eaa5346f3))

## [0.8.27](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v0.8.27) (2025-07-23)

### Изменения

- Опции для логина, полуофлайн режим ([0445348](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/04453483dc4fe56963de202c051db2fa8b55ee72))

## [0.8.25](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v0.8.25) (2025-07-17)

### Изменения

- Модули ИИ ассистентов и распознавания показаний ([4d99ed2](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/4d99ed22d9e38e878cff6bd0c2eb17ca134261de))
- Новые компоненты из МК ([0a983f0](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/0a983f03aa5cc97963edb806c0d3808166d3f05a))
- Обновление сетки ([1aa305d](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/1aa305d4853781a098bc76fdf9bc94eb52dafe0d))
- Перенос классов офлайна в платформу ([71b648c](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/71b648c4ce50e220a8f22085bfc60c885d05cfad))
- Up ([a1d0c33](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/a1d0c33b9e68b06701b592541e5a519467565b8b))

## [0.8.22](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v0.8.22) (2025-06-11)

### Изменения

- Архивные сессии ([4ea5f90](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/4ea5f9071667f043457573bafab631d9301681ef))
- Добавлена возможность использовать якорные ссылки в компоненте tmobis-nav-tabs ([310a2a5](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/310a2a53e4484cc5607274c8fcd5344a0eeb206c))

### Исправления ошибок

- Изменен текст ошибки голосового ввода на русский ([c0dfbe7](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/c0dfbe7fd1918bafe1075e0f2747051d275f7aec))
- Исправлен голосовой ввод ([e2361fd](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/e2361fd3e7e0ff2964805bd518198bca2571aa37))

## [0.8.20](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v0.8.20) (2025-06-09)

### Изменения

- IP в сессиях ([dbd7449](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/dbd7449d52307137b157c809b2553b18faacd2e2))

### Исправления ошибок

- Изменение внешнего вида голосового ввода ([e67ae9b](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/e67ae9b23ba5158e11f26d163b1ed903dff77ebf))
- Quartz addTrigger ([f1b5927](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/f1b592739870f0400686cb99052c77f4874c605b))
- report ParamsList not required ([3c6b905](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/3c6b9056464a89b64f031e239eb40d4d736b750f))
- **signalr:** retryPolicies ([138dcd6](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/138dcd6080b36b3136f5313e2a44df7cfeb93806))

## [0.8.18](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v0.8.18) (2025-06-04)

### Изменения

- Запрос файлов из сервиса ПФ ([ec37fb5](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/ec37fb5ce9441673538bf490d64dbb03b4791aa6))

### Исправления ошибок

- Возвращение события при аутентификации в history ([bc6814c](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/bc6814c8dc21d4ec744fae71367db5a6f1255c5f))
- Исправление ошибки в проекте переноса файлов ([e4174fa](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/e4174fa652ebf85c46498025dc9726ce918f76e0))

## [0.8.17](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v0.8.17) (2025-06-03)

### Изменения

- Возможность переключения активной вкладки извне для компонента tmobis-nav-tabs ([c13c8fa](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/c13c8fab46e888409f58061767517c725851a408))
- Исправление ответа WebApi ([a8483ad](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/a8483ad0845638fecd01dc3e39ea307a54a7448d))
- Клиент файлового сервиса ([c0f2a96](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/c0f2a96bc2b59225e8ac9ea5b26904a2a2724fca))
- Отдельный файловый сервис для фоновых операций ([ffa02e9](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/ffa02e99eb96bdb86c11e4f602cb3002851350d6))

### Исправления ошибок

- Исправление ошибки конфигурации сервиса ПФ ([5851aa3](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/5851aa387acbce82759f0aa97527e8413735b6cd))
- Правки файлового контрола ([226a2a1](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/226a2a1c4ff0b6046753e8143c43ddeb1d9f5edb))

## [0.8.14](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v0.8.14) (2025-05-31)

### Изменения

- Добавление методов обработки строк в BaseExcelReader ([cb90470](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/cb90470fc36892fe5d44ef4797b9352a52f7fd36))

## [0.8.12](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v0.8.12) (2025-05-28)

### Изменения

- Добавление BaseExcelWriter ([4b79a45](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/4b79a45c3bfb00e471135e5da7620263ece09433))

### Исправления ошибок

- Исправление мобильной верстки логин формы ([4e77c70](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/4e77c708a7ec1c15eb1f0f1041ada140eb614def))

## [0.8.11](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v0.8.11) (2025-05-27)

### Изменения

- Комбинированый датасервис ([cac7146](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/cac71467b624c60ba53af81c3b9b04779a4567b5))
- Массовый реворк Платформы ([f276fd9](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/f276fd9d4a57b4358d2b188df6f258f9f98f676f))
- Модернизировать работу с офлайном в бэкенде ([ce9104f](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/ce9104fbcbd646bc9c786f0c0801040ff10035c6))
- Модернизировать работу с файлами ([b6a2379](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/b6a237999adfab9ed1b468a9af87172e9bf30bae))
- Подключить AuditBigData в Платформу ([7bae202](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/7bae202bb2fde8d00eb7f8d1889f375a6514adfc))
- Продвинутый FlexberryRepository ([bf1de8a](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/bf1de8af59c0f08f24f045bffb417b78b03a733c))
- Слой обмена сообщениями ([7f0aba6](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/7f0aba6db9b6536340435d1f9bcbd69c6c16d1b3)), [#328](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/328)
- Страница отображения ошибок в Development ([d586770](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/d586770c9621e7bd6fd686c216abcc3fbac4a4ce))
- Централизация кода аутентификации на фронтенде и воплощение ([a7a2bcb](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/a7a2bcb7fa95eaaad587444ea7c11c048f0a17d9))

### Исправления ошибок

- Возможность получать пустой ответ по запросу полномочий ([b537056](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/b5370568ab3669757b9e2f4d6d945b1935b0e3d1))
- Изменение неймспейсов моделей бизнес ядра ([6b44052](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/6b44052005ae28cf155213487621023a5f74d71f))
- Исправил ошибки в Startup ([c22a759](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/c22a759bd80b319a4a90f8e135f09a1d31883c71))
- Исправление добавления нового детейла ([7dcf5e5](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/7dcf5e5078ad553cac477bcf2356daf4bc1a3487))
- Исправление материализации типов ([ef8d8b5](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/ef8d8b5ae39e3fac9c2523a55bc2a68e065c7512))
- Исправление мультиюзера в токене ([e6b2626](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/e6b26265ec235880c352a3c09d21c8bd47f8061a))
- Исправление наименования метода в БС ActualEntity ([c0c3c9b](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/c0c3c9b52848965599f7bf90fae5e9dea7009334))
- Исправление ошибки с получением полномочий ([3779644](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/3779644308ad6bcfed32d6622bc0d40aba7a29c4))
- Исправление ошибки с регистрацией ДСов из-за Replace ([da2cce3](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/da2cce35025562aa73ae288489db2852f5694464))
- Исправление ошибок на форме логина ([b76cdff](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/b76cdffcb469ff57022ace28103256244eaa8834))
- Исправление ошибок работы нового хэша паролей ([32ee809](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/32ee809337b4d956005dfb8b496c32d87dc07c77))
- Исправление ошибок с автоинкрементом и IWTown ([48ba7b6](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/48ba7b6b8e9967964f280afd1ecf2a1ff82726b5))
- Исправление потерянного проекта в SLN Платформы ([6c003bd](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/6c003bdad9560c14ee5767403b3d775b8c033022))
- обновление wcf на новой схеме модулей ([13f9c2c](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/13f9c2c92522854e47adacc64efe029fa28ab8b1))
- namespaces и публикация пакетов ([9a441b9](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/9a441b90312cbcae087662e4cb995de04f960c50))
- Update user.js, wrong namespace ([74b3948](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/74b394868ad7489d83ca538aea9e8e4d47663af3))

### Документация

- Чейнджлог версии 0.7.5 ([524337c](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/524337c76972dbd8d4b1e47a5ee8c3f9b5127f58))

## [0.7.5](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/tags/v0.7.5) (2025-03-27)

### Изменения

- Массовый реворк различных сервисов при модернизации МК ([e960b1e](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/e960b1e745cae3032e1034fc9d5295900b986ac6))
- Поднятие версий базовых пакетов ([8fdc592](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/8fdc592330e732376df01ce14375a774e83a687e))
- **back:** Заготовка веб-модуля Flexberry + ExportExcelODataService ([4713b66](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/4713b66023ca1484eca5fe2417b9c7bcbab59786))
- **core:** Основа для модуля подключения Unity DI ([1d028ac](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/1d028acb6291089d8dba46051d858c02484c2aa1))
- **reports:** Доработка контроллера ПФ для бэкендов ([a3c93c0](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/a3c93c0ea166fcfeb1e0b3015486bc39d1b38458))

### Исправления ошибок

- Исправление ошибки ссылки на tmobis.core при сборке на Linux в проекте Unit ([0f2fd39](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/0f2fd39666508191fff0dcc8fdc261d75b40915b))
- Исправление проектов бизнес-периферии ([2b87b9e](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/2b87b9e84403f914e4e1bffe746691e7d45a1cde))
- **back:** Исправление опечатки в названии модуля UnityModule ([94b06b1](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/94b06b17a9a3123ca7d53a50652382577d6090f9))

### Рефакторинг

- Добавить поля аудита в базовый класс DataObject Платформы  ([ad4d71c](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/ad4d71ce3d7bc21d795118068c2f7d37e2086b94)), [#267](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/issues/267)
- Добавлен текст о зависимости MSSQL DataService от настроек окружения ([1ef20f3](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/1ef20f39e93fbd7e108605ec890f284301dd590e))
- Убрана лишняя зависимость в ReportController ([532d29e](https://gitlab.timepath.ru/timepath/platform/tmobis/tmobis.core/-/commit/532d29ec5a3a140ec9ebd09bd5bdd03aa2034f13))

## 0.7.2 (2025-02-18)

