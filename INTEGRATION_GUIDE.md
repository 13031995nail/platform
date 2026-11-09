# Руководство по интеграции с Платформой

Это руководство описывает, как подключить Платформу к внешнему проекту на бэкенде (.NET) и фронтенде (React Native/Expo).

---

## Часть 1: Подключение на .NET Backend

### 1.1 Установка пакетов

Добавьте в `.csproj` только ссылки на пакеты — версии будут взяты из `Directory.Packages.props`:

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <ItemGroup>
    <!-- Ядро платформы -->
    <PackageReference Include="Tmobis.Core" />
    
    <!-- Периферия (примеры) -->
    <PackageReference Include="Tmobis.Data.EF" />
    <PackageReference Include="Tmobis.Data.EF.Postgres" />
    <PackageReference Include="Tmobis.Frameworks.Web" />
    <PackageReference Include="Tmobis.Frameworks.Web.Backend" />
    <PackageReference Include="Tmobis.Business.Core" />
    
    <!-- Аутентификация -->
    <PackageReference Include="Tmobis.Auth.JwtAuth" />
    
    <!-- Слой обмена сообщениями -->
    <PackageReference Include="Tmobis.Medium.WebApi" />
    <PackageReference Include="Tmobis.Medium.RabbitMQ" />
    <PackageReference Include="Tmobis.Medium.Signalr" />
    
    <!-- Сторонние пакеты -->
    <PackageReference Include="Swashbuckle.AspNetCore" />
  </ItemGroup>
</Project>
```

В `Directory.Packages.props` укажите все версии:

```xml
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>
  <ItemGroup>
    <PackageVersion Include="Tmobis.Core" Version="4.4.0" />
    <PackageVersion Include="Tmobis.Data.EF" Version="4.4.0" />
    <PackageVersion Include="Tmobis.Data.EF.Postgres" Version="4.4.0" />
    <PackageVersion Include="Tmobis.Frameworks.Web" Version="4.4.0" />
    <PackageVersion Include="Tmobis.Auth.JwtAuth" Version="4.4.0" />
    <PackageVersion Include="Swashbuckle.AspNetCore" Version="10.1.0" />
    <!-- ... другие пакеты ... -->
  </ItemGroup>
</Project>
```

### 1.2 Настройка корня композиции (DI)

Платформа предоставляет два типа строителей приложений:

| Строитель | Назначение | Используется для |
|-----------|------------|------------------|
| `WebAppBuilder` | Веб-приложения | MVC, Web API, Minimal API, Swagger |
| `WorkerAppBuilder` | Фоновые службы | Бэкграунд джобы, агенты, консольные утилиты |

---

#### Вариант A: Бэкенд для фронтенда (с аутентификацией Keycloak)

Используется, когда бэкенд обслуживает React-фроненд и требуется авторизация пользователей.

```csharp
using ...

var modules = new ModuleRegisterBuilder([
    typeof(WebFrameworkModule),           // База веб-приложения
    typeof(WebBackendModule),             // Текущий пользователь и тд
    typeof(KeycloakAuthModule),           // Аутентификация через Keycloak
    typeof(UserSessionsModule),           // Управление сессиями
    typeof(EntityFrameworkModule),        // EF Core
    typeof(EFPostgresModule),             // Postgres провайдер
    typeof(QuartzAdapterModule),          // Планировщик задач
    typeof(WebApiListenerModule),         // Web API слушатель сообщений
    typeof(ObjectsModule),                // Прикладная объектная модель
]);

var builder = new WebAppBuilder(modules, args);
var app = builder.Build();

await app.RunAsync();
```

---

#### Вариант B: Веб-сервис с API Key (внутренний сервис)

Используется для внутренних сервисов без пользовательской авторизации.

```csharp
using ...

var modules = new ModuleRegisterBuilder([
    typeof(WebFrameworkModule),           // База веб-приложения
    typeof(ApiKeyAuthorizationModule),    // API Key авторизация
    typeof(WebApiListenerModule),         // Web API слушатель сообщений
]);

var builder = new WebAppBuilder(modules, args);
var app = builder.Build();

await app.RunAsync();
```

---

#### Вариант C: Фоновая служба (Worker)

Используется для фоновых задач.

```csharp
using ...

var modules = new ModuleRegisterBuilder([
    typeof(QuartzAdapterModule),          // Планировщик задач
]);

var builder = new WorkerAppBuilder(modules, args);
var app = builder.Build();

await app.RunAsync();
```

---

### 1.3 Модули в платформе

#### IModule — интерфейс модуля

```csharp
public interface IModule
{
    // Регистрация сервисов (в первую очередь)
    void PreRegister(IServiceCollection services, IConfiguration config, IModuleRegister modules);
    
    // Регистрация сервисов (в последнюю очередь)
    void PostRegister(IServiceCollection services, IConfiguration config, IModuleRegister modules);
    
    // Основная регистрация
    void Register(IServiceCollection services, IConfiguration config, IModuleRegister modules);
    
    // Настройка приложения (в первую очередь)
    void PreConfigure(IServiceProvider services, IConfiguration config, IModuleRegister modules);
    
    // Настройка приложения (в последнюю очередь)
    void PostConfigure(IServiceProvider services, IConfiguration config, IModuleRegister modules);
    
    // Основная настройка
    void Configure(IServiceProvider services, IConfiguration config, IModuleRegister modules);
}
```

#### ModuleBase — базовый класс

Используйте `ModuleBase` вместо `IModule` напрямую:

```csharp
public class MyModule : ModuleBase
{
    public override void Register(IServiceCollection services, IConfiguration config, IModuleRegister modules)
    {
        base.Register(services, config, modules);
        
        // Регистрация ваших сервисов
        services.AddTransient<IMyService, MyService>();
        
        // Добавление слушателей и продьюсеров
        services.AddListenedMessage<MyMessage>("WebApi");
        services.AddProducerMessage<MyProducer>("RabbitMQ");
    }
}
```

#### Ключевой метод: Register

**Register** — основной метод регистрации сервисов в DI контейнере. Выполняется в следующем порядке:

1. **PreRegister** — регистрация в первую очередь (например, базовые сервисы)
2. **Register** — основная регистрация
3. **PostRegister** — регистрация в последнюю очередь (например, финализация конфигурации)

```csharp
public override void Register(IServiceCollection services, IConfiguration config, IModuleRegister modules)
{
    base.Register(services, config, modules);
    
    // 1. Конфигурация опций из appsettings.json
    services.Configure<MyOptions>(config.GetSection("MySection"));
    
    // 2. Регистрация сервисов
    services.AddTransient<IMyService, MyService>();
    
    // 3. Добавление слушателей и продьюсеров
    services.AddListenedMessage<MyMessage>("WebApi");
    services.AddProducerMessage<MyProducer>("RabbitMQ");
    
    // 4. Дополнительные настройки
    ...
}
```

**Параметры метода:**
- `services` — `IServiceCollection` — коллекция для регистрации сервисов
- `config` — `IConfiguration` — конфигурация из appsettings.json
- `modules` — `IModuleRegister` — список всех зарегистрированных модулей (для межмодульного взаимодействия)

---

#### Правила регистрации в Платформе

**1. Transient по умолчанию**
Платформа использует `Transient` по умолчанию для большинства сервисов — это позволяет избежать проблем с состоянием и делает тестирование проще. Используйте `Scoped` и `Singleton` только когда это действительно необходимо.

```csharp
// Transient — по умолчанию (новый экземпляр каждый раз)
services.AddTransient<IMyService, MyService>();

// Scoped — один экземпляр на запрос (используйте с осторожностью)
services.AddScoped<IRepository, MyRepository>();

// Singleton — один экземпляр на всё приложение (используйте редко)
services.AddSingleton<IEntityRegister, EntityRegister>();
```

**2. Переопределение регистраций через AddOrReplace...**
Когда модуль должен переопределить регистрацию из другого модуля, используйте `AddOrReplace*` методы вместо обычных `Add*`. Это позволяет избежать конфликтов при изменении порядка подключения модулей.

```csharp
// Платформенные методы расширения для переопределения
services.AddOrReplaceTransient<IMyService, MyService>();
services.AddOrReplaceScoped<IRepository, MyRepository>();
services.AddOrReplaceSingleton<IEntityRegister, EntityRegister>();

// Для ключевых сервисов
services.AddOrReplaceKeyedTransient<IMessageProducer, RabbitProducer>("Rabbit");

// Для маппингов сообщений
services.AddOrReplaceListenedMessage<MyMessage>("WebApi");
services.AddOrReplaceProducerMessage<MyMessage>("RabbitMQ");
```

**Примеры из реальных модулей:**

| Модуль | Переопределяемый сервис | Метод |
|--------|------------------------|-------|
| `EFPostgresModule` | `IDbContextConfigurator` | `AddOrReplaceTransient` |
| `WebBackendModule` | `ICurrentUserProvider` | `AddOrReplaceScoped` |
| `KeycloakAuthModule` | (новый сервис) | `AddTransient` |
| `FlexberryModule` | `IEntityRegister`, `IRawRepository` | `AddOrReplaceSingleton`, `AddOrReplaceTransient` |
| `CoreModule` | `ICacheProvider` | `AddOrReplaceTransient` |

**3. Регистрация консюмеров сообщений**
Консюмеры регистрируются автоматически через `AddConsumerModule` или вручную:

```csharp
// Автоматическая регистрация из IConsumerModule
services.AddListenedMessage<MyMessage>("WebApi");

// Модуль ConsumerModule автоматически найдёт IMessageConsumer<T>
// и зарегистрирует их как Transient
```

**4. Конфигурация через Options**
Все настройки должны сниматься из конфигурации через pattern Options:

```csharp
// В модуле
services.Configure<RabbitOptions>(configuration.GetSection(RabbitOptions.Rabbit));

// В коде
public class MyService
{
    public MyService(IOptions<RabbitOptions> options)
    {
        var host = options.Value.Host;
    }
}
```

#### Методы расширения ServiceCollection

Платформа предоставляет расширения для `IServiceCollection` в namespace `Tmobis.Core.DI`:

| Метод | Назначение                                    |
|-------|-----------------------------------------------|
| `AddOrReplaceTransient<TI, TC>()` | Заменить сервис, если уже зарегистрирован     |
| `AddOrReplaceScoped<TI, TC>()` | Заменить scoped-сервис                        |
| `AddOrReplaceSingleton<TI, TC>()` | Заменить singleton-сервис                     |
| `AddOrReplaceKeyedTransient<TI, TC>(key)` | Заменить сервис с ключом                      |
| `AddOrReplaceListenedMessage<TMessage>(name)` | Заменить маппинг слушателя                    |
| `AddOrReplaceProducerMessage<TMessage>(name)` | Заменить маппинг продюсера                    |
| `AddListenedMessage<TMessage>(name)` | Добавить маппинг слушателя                    |
| `AddProducerMessage<TMessage>(name)` | Добавить маппинг продюсера                    |
| `GetService<TI>()` | Получить описание зарегистрированного сервиса |

**Пример использования:**

```csharp
using Tmobis.Core.DI;

public class MyModule : ModuleBase
{
    public override void Register(IServiceCollection services, IConfiguration config, IModuleRegister modules)
    {
        services.AddOrReplaceTransient<ICacheProvider, MyCacheProvider>();
    }
}
```

#### Типы модулей

| Интерфейс | Назначение | Примеры |
|-----------|------------|---------|
| `IModule` | Базовый модуль DI без дополнительных возможностей | `CoreModule`, `BusinessCoreModule` |
| `IEntityModule` | Модуль объектной модели — регистрирует сущности и миграции EF | `CustomerModule`, `WorkShiftModule`, `UnitModule` |
| `IEntityModuleExternal` | Модуль внешних сущностей (из сторонних библиотек) | `FlexberryModule` |
| `IConsumerModule` | Модуль с консюмерами сообщений (`IMessageConsumer<T>`) | `AdmissionCommitteeModule`, `EmailConsumerModule` |
| `IMessageModule` | Модуль с продюсерами и слушателями сообщений | `MeterValueDetectionModule`, `GigachatModule` |
| `IProducerModule` | Модуль с продюсерами сообщений | `RabbitProducerModule`, `DelayedMessageCreatedRepositoryProducerModule` |
| `IWebPipelineModule` | Модуль веб-pipeline — настраивает middleware ASP.NET Core | `WebFrameworkModule`, `SwaggerModule`, `SignalrModule` |
| `IWebOptionsModule` | Модуль настроек MVC — `Configure(MvcOptions)` | `WebFrameworkModule` |
| `IODataModule` | Модуль OData — настраивает OData endpoint | `FlexberryODataAdapterModule` |

---

**Группировка по назначению:**

**1. Ядро платформы:**
- `IModule` — база
- `IEntityModule` / `IEntityModuleExternal` — объектная модель

**2. Обмен сообщениями:**
- `IConsumerModule` — консумеры
- `IMessageModule` — сообщения
- `IProducerModule` — продьюсеры

**3. Веб-технологии:**
- `IWebPipelineModule` — middleware и pipeline
- `IWebOptionsModule` — MVC опции
- `IODataModule` — OData endpoints

**4. Комбинации:**

Многие модули реализуют несколько интерфейсов одновременно:

```csharp
// Модуль с сущностями и консюмерами
public class WorkShiftModule : ModuleBase, IEntityModule, IConsumerModule, IMessageModule

// Модуль с сущностями и продюсерами
public class EmailModelModule : ModuleBase, IEntityModule, IMessageModule

// Модуль веб-инфраструктуры
public class SignalrModule : ModuleBase, IWebPipelineModule, IProducerModule
```

### 1.4 Объекты данных (Entities)

#### Интерфейсы сущностей

В платформе сущности описываются через интерфейсы. **Всегда используйте интерфейс для работы с сущностями**, а конкретные классы — это внутренняя реализация (для ORM в первую очередь).

| Интерфейс | Назначение                                                                   |
|-----------|------------------------------------------------------------------------------|
| `IEntity` | Базовый интерфейс — свойство `Id: Guid` и аудит                              |
| `IAuditable` | Аудит: `CreateTime`, `Creator`, `EditTime`, `Editor`                         |
| `IBusinessEntity` | Бизнес-сущность — используется для бизнес-логики (использовать по умолчанию) |
| `IStoredEntity` | Сохраняемая сущность — используется для хранения в БД (в особых случаях)     |
| `IRegisteredEntity` | Маркер для регистрации конкретной сущности                                   |

```csharp
using Tmobis.Core.Models;
using Tmobis.Core.Entities;

// Интерфейс сущности (используйте для работы)
public interface IMyEntity : IBusinessEntity, IRegisteredEntity
{
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
}

// Класс сущности — автогенерируется или ручная реализация
// Всегда приводите к интерфейсу при работе
public class MyEntity : BusinessEntity, IMyEntity { ... }
```

#### View — контроль над выборкой данных

**View (представление)** — определяет, какие связанные сущности загружаются при запросе,
Яа также служит для определения полей для поиска.

```csharp
public interface IMyEntity : IBusinessEntity, IRegisteredEntity
{
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Description { get; set; }
    public ICustomer Customer { get; set; }  // Связанная сущность
    
    public static class Views
    {
        // Включает связанный Customer
        public static readonly EntityView WithCustomer
            = new(nameof(WithCustomer), new HashSet<string> { "Customer" });
    }
}
```

**Свойства EntityView:**
- `References` — связанные сущности для загрузки (`Include` в EF)
- `SearchProperties` — свойства для поиска
- `ViewName` — имя представления

**Пример использования:**

```csharp
public class MyService
{
    private readonly IRepository repository;
    
    public async Task<IMyEntity?> GetWithDetailsAsync(Guid id)
    {
        // Загрузка с представлением WithCustomer
        return await repository.GetByIdAsync<IMyEntity>(id, view: IMyEntity.Views.WithCustomer);
    }
}
```

#### Регистрация сущностей

```csharp
public class MyObjectsModule : ModuleBase, IEntityModule
{
}
```

### 1.5 Настройка appsettings.json

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;User ID=postgres;Password=pass;Port=5432;database=mydb;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "KeycloakAuth": {
    "KeycloakIssuer": "https://keycloak.example.com/realms/myrealm",
    "KeycloakAudience": "account",
    "OpenIdConfigUrl": "https://keycloak.example.com/realms/myrealm/.well-known/openid-configuration"
  },
  "Rabbit": {
    "Host": "localhost",
    "User": "guest",
    "Password": "guest",
    "VirtualHost": "/"
  }
}
```

### 4.1 Регистрация репозитория

```csharp
// В модуле
public override void Register(IServiceCollection services, IConfiguration config, IModuleRegister modules)
{
    base.Register(services, config, modules);
    
    services.AddTransient<IMyRepository, MyRepository>();
}
```

### 4.2 Использование репозитория

```csharp
public class MyService
{
    private readonly IRepository<IMyEntity, Guid> _repository;
    
    public MyService(IRepository<IMyEntity, Guid> repository)
    {
        _repository = repository;
    }
    
    public async Task<List<IMyEntity>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }
    
    public async Task<IMyEntity?> GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }
    
    public async Task SaveAsync(IMyEntity entity)
    {
        await _repository.AddOrUpdateAsync(entity);
        await _repository.SaveChangesAsync();
    }
}
```

---

## Часть 2: Подключение на React Native/Expo

### 2.1 Установка пакетов

```bash
yarn add @tmobis/core @tmobis/core-business
yarn add @tmobis/auth-keycloak @tmobis/business-process-workshift
yarn add @tmobis/signalr @tmobis/notification
```

### 2.2 Настройка корня композиции

Создайте `index.tsx`:

```tsx
import { registerRootComponent } from 'expo';
import { ExpoRoot } from 'expo-router';
import ModuleBase from "@tmobis/core/modules/module";
import CoreModule from '@tmobis/core/modules/core-module';
import AppModule from "@/modules/app-module";

import BusinessCoreModule from '@tmobis/core-business/module';
import WorkShiftModule from '@tmobis/business-process-workshift/module';
import { initializeModulesStatic, getRouteImports } from '@tmobis/core/modules/composition-root';
import { ModulesProvider } from '@tmobis/core/modules/provider';
import { AuthProvider } from '@tmobis/auth-keycloak/contexts/AuthProvider';
import KeycloakModule from "@tmobis/auth-keycloak/module";

const modules: ModuleBase[] = [
  new CoreModule(),
  new AppModule(),
  new BusinessCoreModule(),
  new WorkShiftModule(),
  new KeycloakModule(),
];

// Инициализация статических данных
initializeModulesStatic(modules);

function App() {
  const appCtx = require.context('./src/app');
  const moduleRoutes = getRouteImports(modules)
    .map(value => prefixContext(value, '(tabs)'));
  const ctx = mergeContexts([appCtx, ...moduleRoutes]);

  return (
    <AuthProvider>
      <ModulesProvider modules={modules}>
        <ExpoRoot context={ctx} />
      </ModulesProvider>
    </AuthProvider>
  );
}

registerRootComponent(App);
```

### 2.3 Модули в платформе

#### ModuleBase — интерфейс модуля

```typescript
export default abstract class ModuleBase {
  // Локализация
  locales?: Resource;
  
  // Импорты роутов из модуля
  getRouteImports?(): RequireContext[];
  
  // Регистрация экранов
  getScreens?(): ScreenData[];
  
  // Элементы сайдбара
  getDrawerItems?(): DrawerGroupItemsConfig[];
  
  // Расширения полей форм
  getFieldExtensions?(): FieldExtension[];
  
  // Доп. инициализация
  initStatic?(): void;
  
  // Расширения форм списков
  getListFormExtensions?(): ListFormExtension[];
  
  // Действия в хедере
  getHeaderActions?(): ReactNode[];
  
  // Плитки рабочего стола
  getDesktopItems?(): DesktopItem[];
  
  // Полное переопределение плиток
  getDesktopItemsOverride?(): DesktopItem[];
  
  // Настройка сетки рабочего стола
  getDesktopConfig?(): Partial<DesktopLayoutConfig>;
  
  // Обертка провайдеров
  wrapProvider?(children: ReactNode): ReactNode;
  
  // Жизненный цикл
  initialize?(context: ModuleLifecycleContext): void | Promise<void>;
  dispose?(): void | Promise<void>;
}
```

### 2.4 Создание модуля приложения

Создайте `src/modules/app-module.tsx`:

```tsx
import ModuleBase from "@tmobis/core/modules/module";
import ScreenData from "@tmobis/core/routes/screen-data";
import { Ionicons } from '@expo/vector-icons';
import { Resource } from "i18next";
import FieldExtension, { serializeSlot } from "@tmobis/core/components/entity-edit/field-extension";
import { TmobisInput } from "@tmobis/core/components/input";
import type { DesktopItem, DesktopLayoutConfig } from '@tmobis/core/desktop/types';

export default class AppModule extends ModuleBase {
  // Локализация
  locales: Resource = {
    ru: {
      translation: {
        hello: 'Привет',
        // ...
      }
    }
  };

  // Регистрация экранов
  getScreens(): ScreenData[] {
    return [
      {
        route: 'index',
        label: 'Главная',
        group: 'Общее',
        primary: true,
        priority: 0,
        icon: ({ size, color }) => (
          <Ionicons name="home-outline" size={size} color={color} />
        ),
      },
      {
        route: 'users',
        label: 'Пользователи',
        group: 'Общее',
        icon: ({ size, color }) => (
          <Ionicons name="people-outline" size={size} color={color} />
        ),
      },
    ];
  }

  // Расширения полей
  getFieldExtensions(): FieldExtension[] {
    return [{
      route: 'users',
      order: 1,
      name: 'customField',
      component: () => (
        <TmobisInput
          fieldName="customField"
          label="Пользовательское поле"
        />
      ),
      slot: serializeSlot({ region: 'leftColumn', position: 'top' })
    }];
  }

  // Плитки рабочего стола
  getDesktopItems(): DesktopItem[] {
    return [
      {
        id: 'work-shifts',
        caption: 'Смены',
        href: '/work-shifts',
        order: 1,
        icon: ({ size, color }) => (
          <Ionicons name="calendar-outline" size={size} color={color} />
        ),
      },
    ];
  }
}
```

### 2.5 Использование CompositionRoot

```typescript
import { useModules } from '@tmobis/core/modules/provider';

function MyComponent() {
  const { compositionRoot } = useModules();
  
  // Получить экраны
  const { screens, groups } = compositionRoot.getScreens();
  
  // Получить навигационный план
  const { primary, secondary, secondaryGroups } = compositionRoot.getNavigationPlan();
  
  // Получить расширения полей
  const fieldsByRoute = compositionRoot.getTabsWithExtensions('users');
  
  // Получить плитки рабочего стола
  const desktopItems = compositionRoot.getDesktopItems();
  
  // Получить диспетчер сообщений
  const messageDispatcher = compositionRoot.getMessageDispatcher();
  
  // Отправить сообщение
  messageDispatcher.send('MyMessage', { data: 'value' });
  
  return <View>...</View>;
}
```

---

## Часть 3: Использование сообщений (Mediator Pattern)

### 3.1 Базовая концепция сообщений

Сообщения в платформе — это неизменяемые объекты данных, которые используются для (а)синхронного взаимодействия между компонентами через паттерн Mediator.

Базовый класс для сообщений:

```csharp
public abstract record Message : IMessage
{
    public Dictionary<string, object?> Context { get; init; } = [];
}
```

### 3.2 Категории сообщений

#### По области действия:

| Категория | Описание | Использование |
|-----------|----------|---------------|
| **Domain** | Внутренние сообщения платформы | Логика приложения внутри одного процесса |
| **Remote** | Сообщения для взаимодействия между процессами | Web API, RabbitMQ, SignalR |

#### По семантике (CQRS-стиль):

| Тип | Описание | Возврат значения | Примеры |
|-----|----------|------------------|---------|
| **Command** | Инструкция выполнить действие | `IMessage<T>` | `ChangePassword`, `CreateOrder` |
| **Query** | Запрос данных | `IMessage<T>` | `GetUserById`, `GetOrdersList` |
| **Event** | Уведомление о событии | `IMessage` (void) | `OrderCreated`, `UserLoggedOut` |

### 3.3 Базовые интерфейсы и классы

**Классы для наследования (в `Tmobis.Core.Medium.Models`):**

| Класс | Назначение |
|-------|------------|
| `Message` | Базовый класс без результата |
| `Message<TResult>` | Базовый класс с результатом |
| `DomainEvent` | Доменное событие |
| `DomainCommand<TResult>` | Доменная команда |
| `DomainQuery<TResult>` | Доменный запрос |
| `RemoteEvent` | Удалённое событие |
| `RemoteCommand<TResult>` | Удалённая команда |
| `RemoteQuery<TResult>` | Удалённый запрос |

### 3.4 Создание сообщений

**Domain-сообщение (внутри приложения):**

```csharp
using Tmobis.Core.Medium.Models;

// Команда с результатом
public record StartWorkShiftTask : DomainCommand<StatusMessageResult>
{
    public Guid UserId { get; init; }
}

// Событие (без результата)
public record WorkShiftStarted : DomainEvent
{
    public Guid WorkShiftId { get; init; }
}

// Запрос с результатом
public record GetTodayWorkShiftId : DomainQuery<GetTodayWorkShiftIdResult>
{
    public Guid UserId { get; init; }
}
```

**Remote-сообщение (для Web API/RabbitMQ/SignalR):**

```csharp
// Команда для удалённого вызова
public record SendEmail : RemoteCommand<SendEmailResult>
{
    public string To { get; init; }
    public string Subject { get; init; }
    public string Body { get; init; }
}

// Событие для отправки уведомления
public record SendNotification : RemoteEvent
{
    public string Title { get; init; }
    public string Message { get; init; }
}
```

### 3.5 Регистрация консумеров

Консумер обрабатывает конкретный тип сообщения:

```csharp
public class StartWorkShiftConsumer : IMessageConsumer<StartWorkShiftTask>
{
    public async Task ConsumeAsync(StartWorkShiftTask message)
    {
        ...
    }
}
```

**Регистрация в модуле:**

```csharp
public class MyModule : ModuleBase, IConsumerModule, IMessageModule
{
    public override void Register(IServiceCollection services, IConfiguration config, IModuleRegister modules)
    {
        // Регистрация слушателя ( mapping сообщения на слушатель )
        services.AddListenedMessage<StartWorkShiftTask>("WebApi");
        services.AddListenedMessage<SendEmail>("RabbitMQ");
        
        // Консумеры регистрируются автоматически из IConsumerModule
    }
}
```

### 3.6 Отправка сообщений

```csharp
public class MyService(IMessageDispatcher dispatcher)
{    
    public async Task StartWorkShiftAsync(Guid userId)
    {
        var result = await dispatcher.SendAsync(new StartWorkShiftTask { UserId = userId });
    }
    
    public async Task NotifyAsync(string title, string message)
    {
        await dispatcher.SendAsync(new SendNotification { Title = title, Message = message });
    }
    
    public async Task<Guid> GetWorkShiftIdAsync(Guid userId)
    {
        var result = await dispatcher.SendAsync(new GetTodayWorkShiftId { UserId = userId });
        return result.Id;
    }
}
```

### 3.8 Frontend

На frontend сообщения сериализуются и отправляются через медиумы (SignalR, WebApi):

```tsx
import { useMessageDispatcher } from '@tmobis/core/modules/provider';

function MyComponent() {
  const dispatcher = useMessageDispatcher();
  
  // Отправка сообщения
  const handleSend = async () => {
    await dispatcher.send('SendNotification', {
      title: 'Уведомление',
      message: 'Новое событие'
    });
  };
  
  return <Button onPress={handleSend}>Send</Button>;
}
```

---

## См. также

- `REPO_STRUCTURE.md` — структура репозитория
- `AGENTS.md` — инструкции для агентской работы
- `src/templates/back-ef/` — пример backend проекта
- `src/templates/expo-app/` — пример frontend проекта
