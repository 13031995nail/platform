# Генератор сущностей (EntityGenerator)

Инструмент для автоматической генерации классов сущностей из интерфейс-сущностей.

## Использование

```bash
EntityGenerator generate --assembly <путь к сборке> [--output <каталог>]
```

### Параметры

- `--assembly` — обязательный путь к DLL файлу приложения или его объектной модели,
размещенный в папке со всеми собранными в dll зависимостями
- `--output` — необязательный путь к каталогу для выходных файлов (по умолчанию: `./Entities/Generated`)

### Примеры

```bash
# Генерация сущностей для прикладного проекта
entitygen generate --assembly ./src/Application/bin/Debug/net10.0/Application.dll

# Указание кастомного каталога вывода
entitygen generate --assembly ./bin/Application.dll --output ./src/Application/Entities/
```

## Принцип работы

Генератор ищет интерфейсы в указанной сборке и ее зависимостях,
которые реализуют интерфейс `IRegisteredEntity` из `Tmobis.Core`.
Для каждой найденной группы интерфейсов создается отдельный класс сущности.

### Структура сущности

Сгенерированный класс:

- Наследуется от базового класса (`BusinessEntity`, `ActualEntity` или `Dictionary` в зависимости от базового интерфейса)
- Реализует все интерфейсы, наследующие от `IRegisteredEntity`
- Содержит все свойства, определенные в интерфейсах
- Поддерживает атрибуты свойств и типов (EF Core, Newtonsoft.Json и др.)
- Поддерживает вычисляемые свойства через атрибут `[ComputedProperty]`

## Генерация вычисляемых свойств

Для свойств, которые вычисляются на основе стороннего метода, реализован атрибут `ComputedProperty`:

```csharp
[NotMapped]
[ComputedProperty("Tmobis.Business.Core.Models.Employees.EmployeeLogic.Fio")]
string Fio { get; }
```

Генератор создаст свойство вида:

```csharp
public virtual System.String Fio => Tmobis.Business.Core.Models.Employees.EmployeeLogic.Fio(this);
```

Это может (и, скорее всего, должен) быть полный путь, включающий неймспейс и имя класса.
Обычно, для вычислимых свойств используются статические классы со статическими полями рядом с интерфейсами,
в которых размещается требуемая бизнес-логика.

```csharp
public static class EmployeeLogic
{
    public static string Fio(IEmployee employee) => $"{employee.LastName} {(string.IsNullOrWhiteSpace(employee.FirstName)
        ? string.Empty
        : char.ToUpper(employee.FirstName[0])
          + "." + (string.IsNullOrWhiteSpace(employee.Patronymic)
              ? string.Empty
              : char.ToUpper(employee.Patronymic[0]) + "."))}".Trim();
}
```

## Атрибуты

Генератор автоматически копирует атрибуты с интерфейсов на класс и свойства (кроме системных атрибутов из `System.*`).
Проверяется `AttributeUsage` — атрибуты копируются только если они применимы к классам.