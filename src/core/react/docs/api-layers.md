# HTTP/API архитектура

## Основные зависимости

```
CompositionRoot (создаёт один ApiClient)
├── CoreModule → ApiClientProvider → React hooks
├── KeycloakModule → configureApiAuth(client)
├── AuthProvider(client) → authClient.login(client)
└── MessageDispatcher → WebApiMessageProducer(MessageApi)

UI → React hooks → API services → ApiClient → Axios → backend
                ↘ Query (cache, retry, AbortSignal)
```

- `api/create-api-client.ts`: транспорт, установка токена, нормализация `ApiError`, **без кеша и React**.
- `api/services/*`: `createEntityApi(client)`, `createMessageApi(client)`, `createFileApi(client)`. Не импортируют React и не создают глобальных клиентов.
- `api/hooks/*`: React Query, состояния запросов и мутаций, получение конкретного сервиса и отмена запросов.
- `api/query/*`: политика retry, отмена прежних query через `AbortSignal`, реактивная изоляция кеша по клиенту и идентичности сеанса.
- `messages/dispatcher.ts`: выбор producer без привязки к Axios.
- `modules/composition-root.ts`: владелец HTTP-клиента и точки сборки модулей.
- `periphery/tech/auth/keycloak/react`: настройка авторизации у клиента композиции. Refresh токена того же пользователя не вызывает сброс кеша, logout/login и смена личности вызывают отмену прежних запросов.
- `periphery/tech/notifications`, `periphery/tech/reports`: использование фабричных сервисов и Query scope вместо singleton.

## Контракты

Транспортные функции не экспортируются в виде default singleton. Для вызовов вне React создайте сервис: `createEntityApi(client)`. Клиент для приложения создаётся в `CompositionRoot`; `ModulesProvider` и `AuthProvider` получают его от одного root.

Кеш разделяется по `['api-client', clientId, revision, ...domainKey]`. При изменении сеанса QueryProvider отменяет и удаляет только прежнюю ревизию, не затрагивая параллельно запущенные запросы новой.
