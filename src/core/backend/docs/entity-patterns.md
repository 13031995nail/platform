# Проектирование сущностей

## IEntity

Базовый интерфейс сущности

### Назначение

`IEntity` - это базовый интерфейс всех сущностей в системе, который определяет:

1. **Идентификатор** - свойство `Id` типа `Guid`
2. **Контракт сущности** - минимальный набор обязательных свойств
3. **Регистрацию** в DI-контейнере для модульности через `IEntityModule`
4. **Тип сущности** - `Business` (основной) и `Stored` (специфичные ORM)

| Тип | Назначение | Пример использования            |
|-----|------------|---------------------------------|
| **Business** | Основной тип сущности для бизнес-логики | Бизнес-логика, Entity Framework |
| **Stored** | Специфичный тип для ORM с отдельными классами | Flexberry (DataObject)          |

### Использование в DI

Интерфейсы `IEntity` и их реализации `Entity` регистрируются в контейнере через модуль `IEntityModule`:

```csharp
// Регистрация основного (business) типа сущности - обязательная
AddOrReplaceKeyedTransient<I(Entity), (Entity)>(nameof(IBusinessEntity));

// Регистрация специфичного (stored) типа сущности - опциональная
AddOrReplaceKeyedTransient<I(Entity), (Entity)>(nameof(IStoredEntity));
```

Для регистрации в DI сущность должна НАПРЯМУЮ наследовать от маркерного интерфейса `IRegisteredEntity`.
Если его нет (просто `IEntity`, `IActualEntity` и тд), то подразумевается, что сущность - абстрактная,
либо это расширение другой сущности (например, `IEmployeeWBirthDate`, наследующий от
`IEmployee`, который в свою очередь наследует от `IRegisteredEntity`).
Такие сущности не регистрируются сами по себе.

### Автоматический маппинг

Маппинг интерфейсов в сущности происходит автоматически под капотом через DI-контейнер.
Маппинг обычно происходит в ORM при CRUD операциях, под капотом бизнес-логики, эндпоинтах работы с сущностями.

### Пример

```csharp
// Интерфейс-сущность
public interface ICustomer : IRegisteredEntity
{
    string Name { get; set; }
    string Email { get; set; }
}

// Реализация
public class Customer : BusinessEntity, ICustomer
{
    public virtual string Name { get; set; }
    public virtual string Email { get; set; }
}
```

### Требования по использованию

Всегда использовать интерфейсы `IEntity` в бизнес-процессах.
Это критично из-за модульности - модули могут доопределять сущности своими свойствами,
и нет гарантии, что конкретный класс сущности вообще будет использоваться в рантайме.

### Примеры

#### EmployeeProvider

```csharp
public class EmployeeProvider(IRepository repository)
    : IEmployeeProvider
{
    public virtual async Task<IEmployee?> GetById(Guid id)
        => await repository.GetById<IEmployee>(id); // Repository зарезолвит конкретный класс

    public virtual async Task<IEmployee?> GetByUser(IUser user)
    {
        ArgumentNullException.ThrowIfNull(user);
        return await repository.FindSingle<IEmployee>(x =>
            x.Actuality && x.UserId == user.Id);
    }
}
```

#### IEmployeeWType и IEmployeeWBirthDate

Расширение сущности в модулях

```csharp
public interface IEmployeeWType : IEmployee, IWEmployeeType { }
```

```csharp
public interface IEmployeeWBirthDate : IEmployee
{
    DateTime BirthDate { get; set; }
}
```

#### EmployeeChanged

```csharp
public class EmployeeChanged(IRepository? repository)
    : OnEntityUpdated<EntityUpdated<IEmployeeWType>, IEmployeeWType>(repository)
{
    protected override async ValueTask<List<IEntity>> ChangeEntity(IEmployeeWType entity)  // Работа с расширенным интерфейсом
    {
        entity.Email = "YAY"; 
        return [];
    }
}
```

---

## IW(Entity)

### Назначение

`IW(Entity)` - это интерфейс-обертка, который:

1. **Хранит ссылку** на сущность через свойство
2. **Определяет отношение** has one между сущностями

### Пример

```csharp
public interface IWCustomer
{
  public ICustomer? Customer { get; set; }
}
```

---

## I(Entity)(Base)

### Назначение

`I(Entity)(Base)` - это интерфейс сущности (где "(Entity)"
= имя конкретной сущности), который:

1. **Определяет сущность** как часть системы (наследует `IEntity`)
2. **Не содержит ссылок** на другие сущности из периферии.
Максимум - сущности из бизнес-ядра.

Может иметь постфикс Base, но лучше называть просто I(Entity),
а расширения - более конкретно (IEmployeeWBirthDate)
или хотя бы с неймспейсом (Tmobis.Overriden1.IEmployee).

Для справочных сущностей это зачастую будет единственной версией сущности
(из-за отсутствия ссылок на другие периферийные сущности).

Для сущностей, имеющих ссылки на другие периферийные сущности, лучше выделять
базовую сущность (без таких ссылок) и сущность-наследника (с ссылками).
Это нужно для гибкости, если есть шанс, что в варианте предметной области может
не быть необходимости иметь данные ссылки - в таком случае можно взять
базовый вариант только с набором собственных полей.

### Примеры

```csharp
public interface IEmployee : IRegisteredEntity, IActualEntity, IWTown
{
    [StringLength(150)]
    string? Email { get; set; }

    [StringLength(50)]
    string? FirstName { get; set; }

    [StringLength(50)]
    string? LastName { get; set; }

    [StringLength(50)]
    string? Patronymic { get; set; }

    [StringLength(50)]
    string? Phone { get; set; }

    string? Photos { get; set; }

    Guid? UserId { get; set; }

    IUser? User { get; set; }

    public class Views
    {
        public static readonly EntityView EmployeeE
            = new(
                nameof(EmployeeE),
                [nameof(Town)],
                searchProperties: [
                    nameof(LastName),
                    nameof(FirstName),
                    nameof(Email),
                ]);
    }
}
```

```csharp
public interface ICustomerBase : IEntity
{
}

public interface ICustomer :
  ICustomerBase,
  IWOrganization
{
}

public interface IWOrganization
{
  public IOrganizationBase? Organization { get; set; }
}
```

