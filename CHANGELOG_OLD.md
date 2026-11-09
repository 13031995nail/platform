## [0.8.8 - 0.8.62 Фронтенд] - Потерянная история
## [0.8.38 - 0.8.57 Бэкенд] - Потерянная история
## [0.8.37 Бэкенд] - 2025-09-16
#### Changed
- BREAKING CHANGE: `InMemoryMediatorModule` перенесен в ядро, пакет `Tmobis.Medium.InMemory` удален
- Добавлен `ModuleRegisterBuilder`, позволяющий строить приложения из списка типов модулей (вместо инстансов)
- Добавлен атрибут `ModuleRequirementAttribute` для построения зависимостей модулей
## [0.8.15-0.8.36] - Потерянная история
## [0.8.14 Бэкенд] - 2025-05-26
### Added
- Добавлен пакет `DataService.Combined`, позволяющий подключать Postgres или MSSQL датасервис в зависимости от строки подключения в конфигурации
- Добавлены пакеты `Postgres` и `MSSQL` для общего кода, связанного с этими СУБД
- В них добавлены методы проверки строк подключения на соответствие СУБД
## [0.8.13 Бэкенд] - 2025-05-15
### Changed
- `GetRoleNamesByUser` переведен в асинхронный режим
### Added
- Добавлены `IUserVerifier`, `PasswordUserVerifier`, `IncarnateUserVerifier` для проверок пользователей при аутентификации в `PasswordAuthController`
## [0.8.8 Фронтенд] - 2025-05-20
### Added
- Класс EventSource для подписки и вызова событий
### Changed
- Объединение под эгидой сервиса `authenticate` всех кусков кода запуска аутентификации
- Часть работы с мультиюзером вынесена в новый сервис `multiuser`
- При воплощении в пользователя также отправляется текущий токен авторизации для проверки пользователя на бэкенде
### Removed
- Удален `armti-authenticate`
## [0.8.6 Фронтенд] - 2025-05-15
### Added
- Добавлен Flexberry BigDataAudit на фронтенде
### Changed
- Ссылки в сайтмапе на аудит теперь ведут на новый аудит
## [0.8.12 Бэкенд] - 2025-05-15
### Added
- Добавлен Flexberry BigDataAudit по умолчанию в `FlexberryModule`
## [0.8.5 Фронтенд] - 2025-05-13
### Changed
- Переименован компонент `photos` в `tmobis-photos` (т.к. не работал с предыдущем названием)
- Рефакторинг `FileUtils`
- Рефакторинг сервиса `file`: добавлены уникальные фичи из МК, внедрен async/await вместо вложенных промисов, приборка по логике методов (получения файлов), работа с новыми полями вместо `Value` и тд
## [0.8.11 Бэкенд] - 2025-05-13
### Added
- Добавлена возможность публиковать сообщение по цепочке с отменой продолжения обработки, с помощью `IChainMessageResult`, `IChainPublishProducer`
- `IChainPublishProducer` реализован для `InMemoryProducer`
- Добавлены сообщения для CUD операций над сущностями - `EntityCreated`, `EntityDeleted`, `EntityUpdated` и результаты для операций - `EntityUpdatedResult`
- Для генерации данных сообщений добавлен абстрактный `EntityBS`, определяющий статус сущности при обновлении и отправляющий нужное сообщение через `PublishChainWithResult`
- Возможность сохранения списка сущностей в `IRepository`
- Для совместимости со старым кодом Flexberry добавлена реализация `ProxyUnityContainer`, проксирующая резолвы к контейнеру в .Net DI
- В сущность `File` добавлены поля `Name`, `Size`, `ExternalId` на основе старой реализации через Flexberry `WebFile.Value` и `IsDeleteProtected` - разрешено ли удалять данный файл через систему
- Добавлен обработчик удаления `File` - `OnFileDeletedConsumer`, проверяющий `IsDeleteProtected` и блокирующий удаление
- Добавлена служба, которая конвертирует файлы в старом формате в новый формат (перекладывает поля из `Value` в новые поля)
### Changed
- Рефакторинг `InMemoryProducer`
### Fixed
- Исправление опечатки `BusinessServicers`
### Removed
- Удалена система nonce для получения файлов
## [0.8.4 Фронтенд] - 2025-05-05
### Added
- Добавлена возможность обрабатывать событие после успешной аутентификации `onAuthSuccessful`
- В модальные окна добавлена возможность передавать колбэк `onHidden`
- Восстановлен сервис `modals` для показа модальных окон без необходимости каждый раз внедрять `applicationController`
- В модальные окна добавлена возможность показать модалку-подтверждение (`showConfirmation`) и модалку-оповещение (`showAlert`)
- Добавлен сервис `model-name-converter`, в него перенесена логика из `model-path-for-type`, добавлен метод `modelNameByClass` для получения имени модели на основе полного типа с бэкенда
### Changed
- Переименован метод `initChecking` в `index` на `redirect`
- В модальных окнах методы `show/hideMdLoading` переименованы в `show/hideLoading`
### Fixed
- Исправлена ошибка с сайтмапом после доработок с ролями
## [0.8.10 Бэкенд] - 2025-05-05
### Added
- Добавлен `PropertyNameConverter` для конвертации имен полей сущностей между фронтендовым и бэкендовым представлениями
- Добавлен тип ответа на сообщение `StatusMessageResult` со встроенным полем `Error`
- Добавлен `FlexberryPredicateConverter` для конвертации LCS-предикатов с фронтенда на бэкенде
- Добавлены модули офлайна: модель (`OfflineModule` и `FlexberryOfflineModule`) и пользовательские сценарии (`FlexberryOfflineUseCasesModule`)
- В модули офлайна перенесены конвертеры, резолверы, объекты выгрузки офлайна из `ARMTI.Tools/BackgroundService`
- В модуль офлайна на основе Flexberry перенесены сообщения-запросы на выгрузку офлайна из бэкенда МК
- В модуль пользовательских сценариев офлайна на основе Flexberry перенесены методы контроллера `UpdatingObjectsController` из бэкенда МК
- Добавлен модуль LZW конвертации, использующийся в реализации офлайна из МК
### Changed
- Добавлены опции кэша `CacheOptions` для кэша сущностей, время жизни по умолчанию 1 час
- Добавлен метод получения типа по полному имени в `EntityRegister`
- Удален `MediumModule`, регистрации из него перенесены в `CoreModule`
### Fixed
- Исправлено определение generic-типа сообщения в `InMemoryProducer`
## [0.8.4 Бэкенд] - 2025-04-22
### Changed
- Возможность удаления, создания, обновления записей в репозитории Flexberry (не протестировано!)
- Новый LINQ обходчик для совместимости с IEntity
- Приборка в коде контроллера пароля
## [0.8.2] - 2025-04-21
### Added
* Шаблон `olv-toolbar` — поле поиска в десктопном тулбаре.
* Компонент `flexberry-menu`:
  * Добавлена возможность использовать кнопку в качестве пункта меню.
  * Добавлен метод `_onSetDefaultSettingItemClick` — клик по пункту меню "Установка по умолчанию" в `colsSettingsItems`.
  * Добавлен метод `_onShowDefaultSettingClick` — клик по пункту меню "Показать установки" в `colsSettingsItems`.
  * Добавлен параметр `enableFilters` — флаг: заблокирована кнопка "Фильтр" или нет.
### Changed
* Компонент `flexberry-menu`:
  * Изменен шаблон и вид принимаемого массива.
* Компонент `olv-toolbar`:
  * Изменен параметр `colsSettingsItems` — пункты контекстного меню для кнопки "Настроить отображение столбцов".
  * Изменен параметр `exportExcelItems` — пункты контекстного меню для кнопки "Настроить отображение столбцов в выгрузке".
  * Изменен шаблон.
* Компонент `flexberry-sitemap-guideline`:
  * Изменен шаблон.
### Fixed
* Компонент `flexberry-menu`:
  * Добавлена проверка наличия `onItemClick`.
## [0.8.1] - 2025-04-18
- Массовый реворк Платформы
### Added
- Добавлен реестр сущностей (`IEntityRegister`) для сбора всех регистрируемых типов сущностей
- Добавлен материализатор сущностей-интерфейсов (`IEntityMaterializer`) между типами сущностей (бизнес <-> хранимые) с возможностью переопределения материализации в модулях
- Добавлены провайдер получения текущего пользователя, класс текущего пользователя, оборачивающий значения из HttpContext и его обертка для технологического текущего пользователя
- Добавлен пакет `tmobis.web.backend` подразумевающий использование в бэкенд сервисах (работающих с пользователями-клиентами) в противовес внутренним веб-службам
- Добавлены методы расширения для переопределения сервисов в `IServiceCollection` и `IServiceProvider`
### Changed
- Отвязаны конкретные реализации от базового серверного кода
  - Убрано использование Flexberry в ядре, выделены отдельные пакеты `tmobis.flexberry.*`
  - Веб вынесен в пакеты `tmobis.web.*`
- Пакет автоинкремента перенесен в раздел Flexberry - `tmobis.flexberry.autoincrement`
- Пакеты аутентификации перенесены в раздел веб
- Все конкретные модели ядра (сущности для основных бизнес-процессов систем: `Employee`, `File`, `User` и тд) вынесены в новый пакет бизнес-ядра в серверном коде
- Введено разделение моделей на интерфейсы (`IEntity`), бизнес-сущности (`IBusinessEntity`), хранимые сущности (`IStoredEntity`) и DTO сущностей (`IDtoEntity`) с возможностью fallback
- Удален `StartupBase` и `ProgramBuilder`, вместо них разработаны `AppBuilderBase`, `WorkerAppBuilder` и `WebAppBuilder`
- `CachedEntityProvider` теперь использует `MemoryCache` вместо `CacheService`
- Удалены реализации `ArmtiDataService`, добавляющие использование `userLimit`
  - Вместо этого разработан `DataServiceFactory`, позволяющий построить ДС с необходимыми настройками
- По умолчанию теперь внедряется `DataService` с `EmptySecurityManager`, ДС с проверкой полномочий внедряется явно только в `OData` (`DataObjectController`)
- Временно отключены бизнес-проекты с сущностями строительства до устаканивания ситуации с моделями бизнес <-> flexberry
- Метод хэширования пароля изменен с технологического SHA256 на .Net PBKDF2
- `TownLimit` перенесен в `tmobis.flexberry.web`
- SQL скрипты перенесены из ядра в бизнес-ядро
### Removed
- Убрана зависимость от Unity DI
## [0.8.0] - 2025-04-07
### Added
* Добавлен роут `acts`.
* Добавлен контроллер `acts`.
* Добавлен роут `dictionaries`.
* Добавлен контроллер `dictionaries`.
* Добавлен шаблон `app-header` для десктопной шапки приложения.
* Добавлен файл для локалей сайтмапа.
* Добавлен файл для локалей кнопок.
* Добавлен компонент `tmobis-modal-dialog` — полная копия `modal-dialog`, переименованный.
* Добавлен компонент `tmobis-thumbnail` — заглушка.
* Добавлен миксин `modal-dialog` — полная копия `modals/modal-dialog`, перемещенный.
* Контроллер `application`:
  * Добавлено свойство `isUserGuideEnabled` — флаг: доступно ли скачивание руководства пользователя или нет.
  * Добавлен метод `downloadUserGuide` — загрузка руководства пользователя.
* Контроллер `edit-form`:
  * Добавлен импорт миксина `common-controller`.
* Контроллер `list-form-common`:
  * Добавлен импорт миксина `common-controller`.
* Контроллер `desktop`:
  * Добавлен импорт миксина `common-controller`.
* Контроллер `settings`:
  * Добавлен импорт миксина `common-controller`.
* Миксин `common-controller`:
  * Добавлен инжект сервисов `appState`, `history`, `modes`, `objectlistviewEventsService`, `session`, `security`, `userSettings`.
  * Добавлены методы `actions.toggleSitemapSidebar`, `actions.logout`.
* Роут `application`:
  * Добавлены методы `actions.commonToggleSitemapSidebar`, `actions.commonHideSitemapSidebar`, `actions.commonLogout`.
* Роут `edit-form-new`:
  * Добавлен импорт миксина `authenticated-route-mixin`.
* Роут `edit-form`:
  * Добавлен импорт миксина `authenticated-route-mixin`.
  * Добавлен инжект сервиса `modes`.
* Роут `list-form`:
  * Добавлен метод `actions.willTransition`.
* Роут `acts`:
  * Добавлен метод `actions.willTransition`.
* Роут `desktop`:
  * Добавлен метод `actions.willTransition`.
* Роут `dictionaries`:
  * Добавлен метод `actions.willTransition`.
* Роут `index`:
  * Добавлен метод `actions.willTransition`.
* Роут `settings`:
  * Добавлен метод `actions.willTransition`.
* Компонент `flexberry-sitemap-guideline`:
  * Добавлена возможность использовать кнопку в качестве пункта меню.
### Fixed
### Changed
* Изменены локали для основных форм приложения.
* Изменены шаблоны `application` и `mobile/application` — шапка вынесена в отдельный файл, в сайтмапе добавлена проверка на авторизацию.
* Изменен шаблон `form-header` для стандартных форм приложения.
* Изменен шаблон `edit-form-header` для форм редактирования.
* Изменен рендер шапки форм для роутов:
  * `detail-edit-form-new`.
  * `detail-edit-form`.
  * `edit-form-new`.
  * `edit-form`.
  * `list-form`.
  * `desktop`.
  * `settings`.
* Роут `edit-form-new`:
  * Изменен метод `actions.willTransition`.
* Роут `edit-form`:
  * Изменен метод `actions.willTransition`.
### Removed
* Удален шаблон `application-modal-dialog`
* Удалены шаблоны `index-header`, `detail-form-header`.
* Удалены компоненты `sitemap/sitemap-node`, `sitemap/sitemap-tree`.
* Удален сервис `sitemap`.
* Удалены миксины `modals/modal-dialog`, `modals/modal-loading`.
* Контроллер `application`:
  * Удалены свойства `hasHeader`, `hasHeaderStyle`, `hasntHeaderStyle`, `style`, `showStyle`.
  * Удалены методы `actions.hideSidebar`, `actions.onChangeSidebar`, `actions.onHideSidebarMenu`, `actions.logout`, `actions.applicationMdOnHidden`.
* Контроллер `detail-edit-form-common`:
  * Удален инжект сервисов `history`, `session`.
* Контроллер `dictionaries-list-form-common`:
  * Удален инжект сервиса `session`.
* Контроллер `edit-form-common`:
  * Удален инжект сервиса `appState`, контроллера `applicationController`.
* Контроллер `edit-form`:
  * Удален инжект сервиса `history`.
* Контроллер `list-form-common`:
  * Удален инжект сервисов `appState`, `history`, контроллера `applicationController`.
* Контроллер `desktop`:
  * Удалено свойство `isReadonlyForm`.
  * Удалено переопределение метода `init`.
* Контроллер `index`:
  * Удалено свойство `greetings`.
  * Удален инжект контроллера `applicationController`.
* Контроллер `login`:
  * Удалены свойства `isReadonlyForm`, `welcomePageLayout`.
  * Удален метод `actions.showLoginForm`.
* Контроллер `settings`:
  * Удален инжект сервисов `userSettings`, `session`.
  * Удален метод `actions.toggleSidebar`.
* Миксин `common-controller`:
  * Удален метод `actions.toggleSidebar`.
* Роут `application`:
  * Удалено свойство `backgroundStyles`.
  * Удалены методы `actions.willTransition`, `actions.attemptLogout`.
* Роут `list-form`:
  * Удален метод `actions.trackPageLeave`.
* Роут `index`:
  * Удален метод `renderTemplate`.
  
## [0.7.5] - 2025-03-27
- Массовый реворк различных сервисов при модернизации МК
### Added
* Разработан `CachedEntityProvider` для работы с кэшируемыми объектами
* Добавлен базовый бэкенд-модуль мобилки - `tmobis.mobile`
* Добавлены опции версионирования веба (`VersionOptions`) в ядре и мобилки (`MobileOptions`) в `tmobis.mobile`
* Добавлена фабрика ДСов для получения разных ДСов (дефолтная реализация через Unity)
* Добавлена папка Liquibase скриптов для MSSQL, добавлен MSSQL драйвер Liquibase
* Добавлен fdg файл для генерации MSSQL скриптов Платформы
* Для модели `Employee` добавлен интерфейс `IEmployee` для переопределения в прикладных проектах
* Добавлен метод расширения `GetByPk` для `IQueryable<DataObject>`
* Разработана основа `EntityMapper` для материализации интерфейсов сущностей
* Добавлен метод в `IModule` для маппинга интерфейсов сущностей в классы сущностей
* Добавлены методы `ConfigurePreContainer` и `ConfigurePostContainer` в `IModule`
### Fixed
* Исправлен запуск юнит тестов фронтенда (изменены настройки testem)
* Исправлена ошибка наименования файла класса `BaseExcelReader`
### Changed
* Разработаны `EmployeeProvider` и `UserProvider` вместо соответствующего кода в классах `CurUserProvider`, `UserBS`, `EmployeeBS`
* Модели `Employee`, `IWEmployee` перенесены в неймспейс `Tmobis.Core.Objects.Employees`
* `TownLimitedAttribute`, `IUserSpecificLimiter` и `UserSpecificLimiter` перенесены в неймспейс `Tmobis.Core.Services.TownLimit`
* Переименованы модели и сериализаторы `employee` на фронтенде, убран префикс у файла, добавлен неймспейс в модель
* Рефакторинг классов в пакете `tmobis.auth.jwtauth` и `tmobis.auth.password` и других классов, использовавших `CurUserProvider`, `UserBS` и `EmployeeBS` для определения сотрудников и пользователей
* Класс `CoreODataFunctions` переименован в `EmployeeFunctions` и перенесен в неймспейс `Tmobis.PrimaryAdapters.OData.Functions`
### Removed
* Удалены классы `EmployeeBS`, `CurrentUserService`
## [0.7.4] - 2025-03-24
### Added
* Добавлен пакет `ember-helpers`.
* Добавлен компонент `tmobis-login-form` — форма входа в приложение.
* Контроллер `login`:
  * Добавлен метод `submitForm`. Срабатывает при нажатии "Войти" и "Работа оффлайн".
  * Добавлен метод `toggleForgotPasswordForm`. Открывает МО "Забыл пароль".
  * Добавлен метод `onForgotPasswordModalHidden`. Необходим для корректного закрытия МО "Забыл пароль".
  * Добавлен метод `toggleSupportForm`. Открывает МО "Служба поддержки".
  * Добавлен метод `onSupportFormModalHidden`. Необходим для корректного закрытия МО "Служба поддержки".
  * Добавлен метод `toggleAboutForm`. Открывает МО "О приложении".
  * Добавлен метод `onAboutFormModalHidden`. Необходим для корректного закрытия МО "О приложении".
* Добавлены локали для формы `login`.
* Компонент `modal-dialog`:
  * Добавлено свойство `stackableButtons` для корректного отображения кнопок в мобильных МО и в МО с классом `mini`.
* Добавлен компонент `tmobis-folder` — стилизованная ссылка на акт/справочник.
* Роут `application`:
  * Добавлен метод `setupSettingsController `для установки пользовательских настроек темы при входе в приложение. Добавлена инициализация свойств контроллера `localeItems`, `themeItems`, `appearanceItems`.
* Сервис `user-settings`:
  * Добавлен метод `saveApplicationUserSetting` для установки общих пользовательских настроек, т.е. без привязки к конкретной странице.
  * Добавлен метод `getApplicationUserSetting` для получения общих пользовательских настроек.
  * Добавлен метод `saveTheme` для установки пользовательских настроек темы.
  * Добавлен метод `setTheme` для изменения ссылки на css-файл с темой.
* Добавлены локали для формы `settings`.
### Fixed
* Компоненты `object-list-view`, `olv-toolbar`, `objectlistview-events` :
  * Поправлен триггер на кнопку "Выбрать все".
### Changed
* Компонент `modal-dialog`:
  * Изменен шаблон компонента для корректного отображения кнопок в мобильных МО и в МО с классом `mini`.
  * При открытии нескольких МО выделяется только одно - последнее открытое.
  * Изменены ссылки на локали для кнопок.
* Добавлены стилевые переменные для компонента `tmobis-folder`.
* Роут `settings`:
  * Изменен метод setupController — удалена инициализация свойств контроллера localeItems, themeItems, appearanceItems.
### Removed
* Удален компонент `login/login-form`.
* Удален компонент `login/login-form-auth`.
* Контроллер `login`:
  * Удален метод `changeVisibleTooltip`.
* Удалены локали для компонента `modal-dialog`.
## [0.5.7] - 2025-01-30
### Added
* Сервис `syncer`:
  * Сервис перенесен из armti-common.
* Утилита `sync-projection`:
  * Утилита перенесена из armti-common.
* Модель `changelog`:
  * Модель перенесена из armti-common.
## [0.5.6] - 2025-01-29
### Added
* Компонент `flexberry-field`:
  * Добавлена передача свойства `dynamicProperties` дочерним компонентам.
* Компонент `olv-toolbar`:
  * Добавлен поиск по списку по кнопке.
* Контроллер `application`:
  * Добавлено свойство `runningOnDevice` - получить тип устройства.
* Компонент `object-list-view-single-column-cell`:
  * Добавлено свойство `columnsToShow` для отображения конкретных полей записи.
* Компонент `object-list-view-row`:
  * Добавлено свойство `columnsToShow` для отображения конкретных полей записи.
### Fixed
* Компонент `flexberry-dropdown`:
  * Поправлено отображение выпадающего меню, когда компонент находится внутри МО (десктоп).
### Changed
* Компонент `flexberry-lookup`:
  * Поправлены наименования методов конфигурирования строки мобильного списка `configure...` -> `configurate...`.
* Контроллер `application`:
  * Изменена структура свойства `headerSitemap`.
## [0.5.5] - 2024-12-16
### Added
* Компонент `olv-toolbar`:
  * Добавлено свойство `_showToolbar` - флаг: скрывать тулбар, если внутри нет видимых элементов.
  * Добавлено свойство `checkAllAtPageLabel` - флаг: показывать лейбл "Выбрать все" у чекбокса или нет.
* Компонент `mobile/flexberry-groupedit`:
  * Добавлено свойство `useCardListView` - флаг: использовать отображение в виде карточек в мобильном списке или нет.
  * Добавлено свойство `showToolbar` - флаг: показывать тулбар или нет.
* Компонент `mobile/flexberry-objectlistview`:
  * Добавлено свойство `useCardListView` - флаг: использовать отображение в виде карточек в мобильном списке или нет.
* Компонент `mobile/object-list-view`:
  * Добавлено свойство `useCardListView` - флаг: использовать отображение в виде карточек в мобильном списке или нет.
* Компонент `mobile/object-list-view-row`:
  * Добавлено свойство `useCardListView` - флаг: использовать отображение в виде карточек в мобильном списке или нет.
  * Добавлен экшн `expandCard` для сворачивания/разворачивания строки списка.
* Компонент `flexberry-lookup`:
  * Добавлена возможность конфигурации строки мобильного списка в `ge-multi-lookup-dialog-content`.
### Fixed
* Компонент `flexberry-dropdown`:
  * Поправлено отображение компонента при свойствах `disabled` и `readonly`.
* Компонент `flexberry-lookup`:
  * Поправлено отображение компонента при свойстве `readonly`.
* Компонент `flexberry-toggler`:
  * Поправлено отображение компонента при свойстве `showCaptions`.
* Компонент `object-list-view-single-column-cell`:
  * Поправлено отображение компонента при свойстве `useCardListView`.
* Поправлено отображение шаблона `loading`.
* Компонент `mobile/flexberry-objectlistview`:
  * Возвращена мобильная пагинация.
* Компонент `mobile/object-list-view`:
  * Возвращена поддержка `sendMenuItemAction` и `menuInRowAdditionalItems`.
### Changed
* Переопределены `classNameBindings` в `flexberry-dropdown`.
  * `readonly:readonly`.
  * `disabled:disabled`.
* Компонент `mobile/flexberry-groupedit`:
  * Отключен метод `setMenuWidth`.
* Компонент `mobile/flexberry-groupedit`:
  * Отключен свойство `allowColumnResize`.
### Removed
* Удалено переопределение компонента `groupedit-toolbar`.
