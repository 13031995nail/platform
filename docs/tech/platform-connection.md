# Подключение Платформы

* Дата создания: 2025-05-18
* Дата обновления:
* Версия: 1.0
* Статус: Реализовано

---

## Контекст

* Позволяет эффективно публиковать пакеты и управлять зависимостями в проектах .NET и NodeJS

## Решение

### Бэкенд

* В каждом модуле платформы прописывается файл `.csproj`:

  ```javascript
  <PropertyGroup>
    <RootNamespace>Tmobis.Business.Common.Contacts.Organization</RootNamespace>
    <PackageId>Tmobis.Business.Common.Contacts.Organization</PackageId>
    <IsPackable>true</IsPackable>
  </PropertyGroup>
  ```
    * `IsPackable` - можно ли упаковать проект в NuGet-пакет
    * `RootNamespace` - корневое пространство имён для проекта или сборки
    * `PackageId` -  название nuget пакета


* Пакеты публикуются в <https://repository.flexberry.org/#browse/browse:timepath-tmobis-nuget>
* При необходимости использования зависимостей из платформы в текущем модуле устанавливаются ссылки на проекты в файле `.csproj` (ProjectReference):

  ```javascript
  <ItemGroup>
    <ProjectReference Include="..\..\..\..\..\..\..\..\core\backend\module\tmobis.core.csproj" />
    <ProjectReference Include="..\..\..\..\..\..\core\backend\module.flexberry\tmobis.business.core.flexberry.csproj" />
    <ProjectReference Include="..\..\..\..\organization.base\tmobis.business.common.contacts.organization.base.csproj" />
    <ProjectReference Include="..\..\..\..\organizationtype\backend\module.flexberry\tmobis.organizationtype.base.flexberry.csproj" />
  </ItemGroup>
  ```

### Фронтенд

* В корневом файле `package.json` указано поле `workspaces` в котором перечислены все относительные шаблоны, указывающие на папки с пакетами:
    ```json
    "workspaces": {
        "packages": [
          "src/tools/*",
          "src/tools/**/*",
          "src/core/frontend",
          "src/core/react",
          "src/periphery/**/*",
          "src/templates/ember-app",
          "src/templates/expo-app"
        ]
      },
    ```
* Yarn создает единую node_modules папку в корне проекта, где устанавливаются все зависимости для всех пакетов
* В корневом файле `lerna.json`  указана настройка для для связки `Yarn Workspaces` и `lerna`
