# Модуль аутентификации Keycloak

Модуль `tmobis.web.auth.keycloak` предоставляет интеграцию с Keycloak для аутентификации пользователей.

## Архитектура

```
┌─────────────┐     ┌──────────────┐     ┌────────────────┐
│   Клиент    │────>│   Keycloak   │────>│  API Backend   │
│ (frontend)  │     │ (IdP)        │     │  (этот модуль) │
└─────────────┘     └──────────────┘     └────────────────┘
```

1. Клиент получает JWT токен от Keycloak
2. Клиент отправляет запрос к API с токеном в заголовке `Authorization: Bearer <token>`
3. Модуль валидирует токен Keycloak
4. Если пользователь не существует, он создается в локальной БД
5. Генерируется локальный JWT токен для работы с API

## Установка

### 1. Добавить проект в решение

### 2. Добавить модуль в основной проект

В корне композиции `Program.cs`:

```csharp
var modules = new ModuleRegisterBuilder([
    // ... другие модули
    typeof(KeycloakAuthModule),
    typeof(JwtAuthModule),
    // ...
```

### 3. Настроить appsettings.json

```json
{
  "KeycloakAuth": {
    "Issuer": "https://keycloak.example.com/realms/myrealm",
    "Audience": "api",
    "OpenIdConfigUrl": "https://keycloak.example.com/realms/myrealm/.well-known/openid-configuration",
    "RealmPublicKey": "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA...",
    "ClientId": "api",
    "EnableUserSync": true,
    "DefaultTownId": "123e4567-e89b-12d3-a456-426614174000",
    "UsernameClaimType": "preferred_username",
    "RolesClaimType": "realm_access.roles"
  },
  "JwtAuth": {
    "Issuer": "TIMEPATHTMOBIS",
    "Audience": "TIMEPATHTMOBIS",
    "Lifetime": "365.00:00:00",
    "IsLifetimeFixed": false,
    "SymmetricKey": "TIMEPATHTMOBISTIMEPATHTMOBISTIMEPATHTMOBISTIMEPATHTMOBISTIMEPATHTMOBIS"
  }
}
```

## Настройка Keycloak

### 1. Создать Realm в Keycloak

1. Войти в админ-панель Keycloak
2. Создать новый Realm (например, `myrealm`)
3. Скопировать URL realm: `https://keycloak.example.com/realms/myrealm`

### 2. Создать Client

1. В Realm перейти в Clients → Create client
2. Выбрать `OpenID Connect`
3. Указать:
   - Client ID: `api`
   - Client type: `openid-connect`
   - Root URL: `https://your-api.com`

### 3. Настроить Credentials

1. Перейти во вкладку Credentials
2. Скопировать Secret (необязательно для этой интеграции)

### 4. Получить Public Key

1. Войти в Realm
2. Перейти в Realm settings → Keys
3. Скопировать Public key (Base64 encoded)
4. Или использовать JWKS endpoint: `https://keycloak.example.com/realms/myrealm/protocol/openid-connect/certs`

## Использование

### 1. Аутентификация через Keycloak

```bash
POST /api/auth/keycloak/token
Authorization: Bearer <keycloak-access-token>
Content-Type: application/json

# Ответ:
{
  "access_token": "jwt-token-from-local-system",
  "userName": "user_login",
  "roles": "admin,user",
  "user": "user-id",
  "town": "town-name",
  "is_old_pwd": false,
  "is_complex_pwd": true,
  "employee": "employee-id",
  "sessionId": "session-id"
}
```

### 2. Обновление токена

```bash
POST /api/auth/keycloak/refresh
Authorization: Bearer <keycloak-access-token>
Content-Type: application/json

# Ответ: новый локальный JWT токен
```

### 3. Получение информации о пользователе

```bash
GET /api/auth/keycloak/me
Authorization: Bearer <keycloak-access-token>

# Ответ:
{
  "login": "user_login",
  "id": "user-id",
  "userId": "keycloak-user-id",
  "userName": "User Name",
  "email": "user@example.com",
  "town": "Town Name",
  "employeeId": "employee-id",
  "lastName": "LastName",
  "firstName": "FirstName",
  "middleName": "MiddleName"
}
```

## Конфигурация

| Параметр            | Описание                         | Обязательный | Значение по умолчанию    |
|---------------------|----------------------------------|--------------|--------------------------|
| `Issuer`            | URL issuer Keycloak              | Да           | -                        |
| `Audience`          | Audience для валидации токена    | Да           | -                        |
| `OpenIdConfigUrl`   | URL discovery document           | Нет          | auto-generated из Issuer |
| `RealmPublicKey`    | Публичный ключ realm (Base64)    | Нет          | -                        |
| `ClientId`          | ID клиента в Keycloak            | Нет          | `api`                    |
| `EnableUserSync`    | Разрешить создание пользователей | Нет          | `true`                   |
| `DefaultTownId`     | ID филиала по умолчанию          | Нет          | -                        |
| `UsernameClaimType` | Claim для логина                 | Нет          | `preferred_username`     |
| `RolesClaimType`    | Claim для ролей                  | Нет          | `realm_access.roles`     |

## Синхронизация пользователей

При первой аутентификации пользователя через Keycloak:

1. Проверяется существование пользователя по логину
2. Если не найден и `EnableUserSync = true`, создается новый пользователь
3. Создается связанный Employee
4. Роли синхронизируются из токена Keycloak

## Примечания

- Для production рекомендуется использовать `OpenIdConfigUrl` для динамического получения ключей
- Для dev-среды можно использовать статический `RealmPublicKey`
- Роли из Keycloak автоматически добавляются пользователю, но не удаляются (чтобы не потерять ручные изменения)
