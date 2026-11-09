# Структура репозитория tmobis.core

## Обзор

`tmobis.core` — монорепозиторий, построенный по принципу **ядро-периферия** с разделением на **технологические** и **бизнес-компоненты**.

```
src/
├── core/                # Ядро платформы
│   ├── backend/         # Бэкенд ядро (.NET)
│   └── react/           # React Native/Expo ядро
├── periphery/           # Периферия (адаптеры и бизнес-логика)
│   ├── tech/            # Технологические модули
│   └── business/        # Бизнес-модули
├── templates/           # Шаблоны и сэмплы
└── tools/               # Внутренние инструменты
```

---

## src/core/ — Ядро платформы

### core/backend — Бэкенд ядро (.NET)

Ядро бэкенда, обеспечивающее базовые абстракции и паттерны:

```
src/core/backend/module/
├── Application/         # Базовые абстракции приложения
├── Audit/               # Аудит событий и изменений
├── Cache/               # Кеширование
├── Converters/          # Конвертеры значений
├── DI/                  # Dependency Injection хелперы
├── Entities/            # Базовые сущности
├── Files/               # Работа с файлами
├── Linq/                # Linq расширения
├── Mappers/             # Мапперы объектов
├── Medium/              # Абстракции слоя обмена сообщениями
├── Models/              # Базовые модели
├── Modules/             # Модули DI (IModule)
├── Passwords/           # Хеширование паролей
├── PrimaryAdapters/     # Первичные адаптеры
├── Reflection/          # Рефлексия и метаданные
├── Repositories/        # Репозитории (IRepository)
├── SecondaryAdapters/   # Вторичные адаптеры
├── Types/               # Типы и константы
└── tmobis.core.csproj   # Основной проект ядра
```

**Ключевые паттерны:**
- `IModule` — модуль DI для регистрации сервисов
- `IRepository`, `IRepositoryFactory` — репозитории
- `IMessage`, `IMessageConsumer` — паттерн медиатор
- Primary/Secondary Adapters — адаптеры для интеграций

---

### core/react — React Native/Expo ядро

React-ядро платформы с компонентами и базовыми абстракциями:

```
src/core/react/
├── src/
│   ├── app/             # Корневой App компонент
│   ├── components/      # UI компоненты (см. ниже)
│   ├── constants/       # Константы (themes, colors)
│   ├── hooks/           # Custom hooks
│   ├── models/          # Базовые сущности
│   ├── modules/         # Модули приложения (CoreModule)
│   ├── routes/          # Маршруты
│   ├── security/        # SecurityProvider, guards
│   ├── storage/         # AsyncStorage адаптер
│   ├── translations/    # i18n
│   └── utils/           # Утилиты: date, debounce, download-file, sort
│
├── docs/                # Документация
├── oapi-gen/            # Генератор OpenAPI клиента
├── package.json         # Зависимости (Gluestack UI, React Native)
├── gluestack-ui.config.json
├── tailwind.preset.js
└── tsconfig.json
```

**Основные компоненты:**
- `alert` — Modal для подтверждений
- `button`, `checkbox`, `input`, `dropdown`, `lookup` — формы
- `datetime` — DatePicker, TimePicker
- `entity-edit`, `entity-list` — CRUD экраны
- `file` — FileInput, FileUploader
- `form-control`, `form-header`, `form-state` — работа с формами
- `header` — Хедер приложения
- `icon`, `more-menu`, `sidebar`, `tab-bar` — навигация
- `toast` — Уведомления
- `ui/` — Базовые UI компоненты (Gluestack UI)

**Модули:**
- `core-module.tsx` — модуль ядра
- `composition-root.ts` — корень композиции, обрабатывающий модули
- `module.ts` — абстракция модуля приложения

---

## src/periphery/ — Периферия

Разделена на **tech** (технологии) и **business** (предметная область).

### periphery/tech — Технологические модули

#### audit — Аудит
- `json/` — JSON модели
- `module/` — DI модули

#### auth — Аутентификация
- `apikey/` — API Key authentication
- `base/` — базовые абстракции
- `jwtauth/` — JWT авторизация
- `keycloak/` — Keycloak интеграция
- `password/` — Password authentication
- `usersessions/` — Управление сессиями
- `windows/` — Windows authentication

#### business-intelligence — BI и отчеты
- `backend/` — Backend реализации
- `frontend/` — Frontend реализации (Ember)

#### cache — Кеширование
- `redis/` — Redis адаптер

#### data — Базы данных
- `db/` — MSSQL, Postgres провайдеры
- `ef/` — Entity Framework адаптеры

#### files — Работа с файлами
- `fileUploader.local/` — локальный загрузчик файлов
- `primaryAdapter/` — контроллер для работы с файлами
- `primaryAdapter.flexberry/` — Flexberry реализация контроллера
- `provider.remote/` — клиент для получения файлов из сервиса
- `s3/` — AWS S3 провайдер
- `service/` — S3 файловый сервис
- `service.flexberry/` — Flexberry файловый севрвис

#### fws — Фреймворки и сервисы
- `flexberry/` — Flexberry (ORM, OData, ...)
- `web/` — Web API адаптеры
- `winservice/` — адаптер для службы Windows

#### import — Импорт данных
- `tmobis.import.excel/` — Excel импорт

#### integrations — Интеграции
- `assistants/` — ИИ чаты/ассистенты
- `email/` — Email (SMTP, MailKit)

#### jobs — Задачи и планировщик
- `quartz/` — Quartz.NET адаптер
- `scheduled/` — плановые джобы

#### logging — Логирование
- `frontend/` — логирование на фронте (Ember)
- `serilog/` — Serilog провайдер
- `Tmobis.Logging.Log4Net/` — Log4Net
- `Tmobis.Logging.Syslog/` — Syslog

#### medium — Слой обмена сообщениями
- `delayed/` — отложенные сообщения
- `inrepo/` — in-memory медиатор
- `rabbitmq/` — RabbitMQ адаптер
- `retried/` — повторные сообщения
- `signalr/` — SignalR адаптер
- `webapi/` — Web API адаптер

#### metrics — Метрики
- `module/` — базовые абстракции
- `opentelemetry/` — OpenTelemetry
- `prometheus/` — Prometheus

#### mobile — Мобильные приложения
- `tmobis.mobile/` — базовые абстракции

#### notifications — Уведомления
- `back/` — бэкенд уведомлений
- `front/` — фронтенд уведомлений

#### offline — Оффлайн режим
- `models/` — базовые модели 
- `usecases/` — сценарии работы

#### reports — Отчеты
- `base/` — базовые абстракции
- `react/` — React адаптер
- `stimulsoft/` — отчеты через Stimulsoft

#### utils — Утилиты
- `lzw/` — LZW сжатие

#### web — Web API
- `primaryadapters/` — WCF адаптер

---

### periphery/business — Бизнес-модули

#### common — Общие бизнес-сущности

```
src/periphery/business/common/
├── address/             # Адресный реестр
│   ├── objectfunction/  # Функция объекта
│   └── workobject.base/ # Объект работы
├── contacts/            # Контакты
│   ├── customer/        # Потребитель
│   ├── customer.base/   # Базовый потребитель
│   ├── organization/    # Организация
│   ├── organization.base/ # Базовая реализация
│   ├── organizationtype/ # Тип организации
│   ├── supplier/        # Поставщик
│   └── supplier.base/   # Базовый поставщик
├── personnel/           # Персонал
│   ├── admissioncommittee/ # Комиссия (2234)
│   ├── placeofemployment/ # Место работы
│   ├── position/        # Должность
│   ├── qualification/   # Квалификация
│   ├── team/            # Команда/бригада
│   └── team.base/       # Базовая команда/бригада
├── resources/           # Ресурсы
│   ├── awp/             # ОЗП
│   ├── tooltype/        # Тип инструмента
│   ├── unit/            # Единица измерения
│   ├── verificationrequierement/ # Требования поверки (2234)
│   └── worktype/        # Тип работы
└── sessions/            # Сессия пользователя
```

#### core — Бизнес-ядро

```
src/periphery/business/core/
├── backend/             # Backend реализации
│   ├── module/          # Бизнес модели
│   ├── module.flexberry/ # Flexberry модели
│   └── tests/           # Тесты
└── react/               # React реализации
    └── src/
        └── models/
            ├── changelog.ts      # Чейнджлог
            ├── employee.ts       # Сотрудник
            ├── liked-folder.ts   # Избранное
            ├── town.ts           # Филиал
            └── user.ts           # Пользователь
```

#### process — Бизнес-процессы

```
src/periphery/business/process/
├── documents/           # Документы
│   ├── actsign/         # Подписи актов
│   ├── awpassessingprogram/ # Акты оценки (2234)
│   ├── awppreparingauditrecord/ # Записи аудита (2234)
│   ├── awpreadinesspassport/ # Паспорта готовности (2234)
│   ├── baseact/         # Базовый акт
│   ├── evaluationsheet/ # Лист оценки (2234)
│   └── redinessact/     # Акты готовности (2234)
├── meters/              # Счетчики
│   └── meterValueDetection/  # Распознавание показаний
├── orders/              # Заказы
├── signs/               # Подписи актов
├── works/               # Работы
└── workshift/           # Смены
```

#### testing — Тестовые модули
- `override1/`, `override1_react/` — Тест переопределений (override)
- `override2/`, `override2_react/` — Второй набор тестов

---

## src/templates/ — Примеры и шаблоны

```
src/templates/
├── back-ef/             # Пример бэкенда на EF
├── back-flexberry/      # Пример бэкенда на Flexberry
├── cache-redis/         # Пример клиента Redis кэша
├── prom-metrics/        # Пример генератора Prometheus метрик
├── gitlab-ci/           # CI/CD конфиги
├── liquibase/           # Пример Liquibase миграций для Flexberry
├── expo-app/            # Пример фронтенд приложения
├── .editorconfig.template # Пример настроек линтера
```

---

## src/tools/ — Инструменты

```
src/tools/
├── conventional-changelog-tmobis/  # Генератор changelog
└── EntityGenerator/       # Генератор бизнес-сущностей
```

---

## Ключевые паттерны архитектуры

### 1. Ядро-Периферия (Ports and Adapters)
- **Ядро** (`src/core/`) — независимые бизнес-абстракции
- **Периферия** (`src/periphery/`) — реализация через адаптеры

### 2. Module Pattern (.NET)
```csharp
public interface IModule
{
    void Register(IServiceCollection services);
}
```

### 3. Mediator Pattern (Message Bus)
```
IMessage -> IMessageConsumer<TMessage>
```

### 4. Repository Pattern
```
IRepository
```

### 5. Dependency Injection
- Каждый проект — модуль с `IModule`
- DI регистрация через `IServiceCollection`

### 6. React Modules
- `IModule` — абстракция модуля приложения
- `CoreModule` — корневой модуль
- Компоненты по папкам по типам (components/, hooks/, utils/)