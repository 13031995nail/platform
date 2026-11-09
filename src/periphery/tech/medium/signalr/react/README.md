# `@tmobis/signalr`

React-модуль SignalR для приложений платформы.

`SignalRProvider` регистрирует в диспетчере производителя только для событий:
события отправляются через SignalR, а запросы и команды — через резервный
производитель WebAPI из `@tmobis/core`.

Пакет предоставляет:

- `SignalRClient` — низкоуровневый клиент поверх `@microsoft/signalr`;
- `SignalRService` — управление сессией и состоянием соединения;
- `SignalRModule` — подключение сервиса к модульной системе `@tmobis/core`;
- `SignalRProvider`, `useSignalR()`, `useSignalRGlobalHandler()` и `useSignalRHandler()` — доступ к готовому клиенту из React-поддерева.

## Подключение модуля

Создайте модуль в app-specific composition root и добавьте его в список модулей:

```ts
import ModuleBase from '@tmobis/core/modules/module';
import {SignalRModule} from '@tmobis/signalr';

export const modules: ModuleBase[] = [
  new AppModule(),
  new SignalRModule(),
];
```

Модуль получает URL из `expoConfig.extra.signalrUrl`. Если отдельный URL не
задан, используется `${expoConfig.extra.backendUrl}/signalr`.
Сам пакет `@tmobis/signalr` не зависит от Keycloak или другой конкретной auth-реализации.

`ModulesProvider` автоматически добавляет `SignalRProvider` для модуля.
Провайдер создаёт сервис после получения `SessionProvider`, регистрирует
производителя сообщений, запускает соединение и освобождает его при удалении.
Ручной `start()` и `stop()` обычно не нужны.

### Синхронизация соединения

Экземпляр `SignalRService` создаётся провайдером один раз, но подключение может повторно запускаться при Strict Mode, Fast Refresh или перемонтировании провайдера.
Кроме того, состояние сессии может измениться, пока асинхронный `start()` или `stop()` ещё не завершился.

- `gateTransition` выполняет изменения состояния соединения последовательно;
- `gateRevision` не позволяет устаревшей операции снова открыть или закрыть соединение;
- `pendingStopWait` отменяет ожидание предыдущей остановки при новом изменении состояния;
- таймаут и повторные попытки `stop()` не дают неуспешной остановке оставить соединение открытым после выхода или очистки.

`dispose()` останавливает соединение и удаляет подписки, созданные сервисом.

## Провайдер сессии

Общий контракт `SessionProvider` находится в `@tmobis/core/auth/session-provider`:

```ts
interface SessionProvider {
  getSession(): Promise<Readonly<{accessToken: string}> | undefined>;
  subscribe(listener: (session: Readonly<{accessToken: string}> | undefined) => void): () => void;
}
```

React-контекст сессии предоставляется через `SessionContextProvider` из
`@tmobis/core/auth/session-context`. Auth-модуль должен подключить свою
реализацию этого контракта; `AuthProvider` из `@tmobis/auth-keycloak` делает
это автоматически. `SignalRProvider` сначала получает абстрактный
`SessionProvider` из контекста, а затем создаёт с ним `SignalRService`.

При отсутствии сессии или токена сервис останавливает соединение.
При появлении новой сессии сервис синхронизирует состояние и запускает клиент с актуальным токеном.

Если остановка соединения завершилась ошибкой, сервис повторяет очистку и может вызвать `onLifecycleError` с событиями:

- `connection-stop-failed`;
- `gate-reconciliation-failed`.

## Отправка сообщений

```ts
import type {MessageMetadata} from '@tmobis/core/models/message';
import type {SignalROutgoingMessage} from '@tmobis/signalr';

class TestMessage implements SignalROutgoingMessage {
  static metadata = {
    type: 'event',
    name: 'TestMessage',
  } satisfies MessageMetadata;

  readonly $type = 'Namespace.Messages.TestMessage';
  declare readonly __messageResult: void;
  context: Record<string, unknown> = {};

  constructor(public text: string) {}
}

await signalRModule.client.send(new TestMessage('Привет'));
```

`send()` принимает только event-сообщение, соответствующее `SignalROutgoingMessage`,
с `static metadata.type = 'event'` и `$type`.
В `$type` передаётся полное имя backend-типа без assembly.
Клиент вызывает hub method `SendMessage` и сериализует сообщение целиком в JSON.

Перед отправкой клиент проверяет metadata и отклоняет query/command с ошибкой,
не вызывая transport. Стандартный platform backend также принимает только типы,
которые зарегистрированы в listened message list и реализуют `IDomainEvent` или `IRemoteEvent`.
Query/command-сообщения из HTTP workflow `@tmobis/core` через SignalR не поддерживаются.

Данные сообщения передаются непосредственно в его теле.
`Context.Target` клиент не заполняет: backend определяет пользователя по текущему SignalR-соединению.

## Приём сообщений

По умолчанию внутренний обработчик клиента подписывается на hub method `ReceiveMessage`.
Именно этот transport method жёстко использует platform backend для `Send()` и `Publish()`,
поэтому в штатной интеграции `receiveMethod` переопределять не нужно.

Backend платформы отправляет через SignalR только `IDomainEvent` и `IRemoteEvent`.
Клиент разбирает данные, извлекает тип и передаёт событие в локальный реестр.

Реестр сначала вызывает единственный глобальный обработчик, зарегистрированный
через `onGlobal()`, а затем прикладные обработчики, зарегистрированные для типа
через `on(type, handler)`.
Типизированная регистрация не создаёт отдельную SignalR-подписку.

В стандартном сценарии тип сообщения определяется по `$type`, который Newtonsoft
добавляет при сериализации на backend. Клиент также умеет принять транспортные
данные в форме `(type, payload)`, но backend платформы этот режим не использует.

Если тип не найден, сообщение не передаётся обработчикам, а ошибка направляется
в `onError`.

## Регистрация обработчиков

### Через `onGlobal`

```ts
const unregister = signalRModule.onGlobal(
  async (payload, context) => {
    console.log(context.type, payload);
  }
);

unregister();
```

Одновременно активен только один глобальный обработчик. Новая регистрация
заменяет предыдущую. Функция `unregister` удаляет только созданную ей регистрацию
и не затрагивает более новый обработчик.

### Через `on`

```ts
const unregister = signalRModule.on<MyMessage>(
  'Namespace.Messages.MyMessage',
  async (payload, context) => {
    console.log(context.type, payload);
  }
);

unregister();
```

`on()` возвращает функцию unregister. Её нужно вызывать для временных подписок.

## React-интеграция

`SignalRModule` автоматически оборачивает React-поддерево в `SignalRProvider`,
когда экземпляр добавлен в `ModulesProvider`:

```tsx
<AuthProvider>
  <ModulesProvider modules={modules}>
    <App />
  </ModulesProvider>
</AuthProvider>
```

`ModulesProvider` должен находиться внутри auth-провайдера, чтобы
`SignalRProvider` получил абстрактный `SessionProvider` из React-контекста.
`SignalRProvider` также экспортируется для нестандартных сценариев, но сам по
себе не управляет модулем: он получает настройки и самостоятельно создаёт
`SignalRService`.

Компонент может получить прикладные зависимости штатными hooks и замкнуть их в callback:

```tsx
function Notifications() {
  const signalR = useSignalR();
  const alerts = useAlerts();

  useEffect(
    () => signalR.on<Notification>(
      'Namespace.Messages.Notification',
      (notification) => alerts.show(notification.text)
    ),
    [signalR, alerts]
  );

  return null;
}
```

Для простых подписок доступен `useSignalRHandler()`. Передавайте в него callback, стабилизированный через `useCallback`, если зависимости не должны вызывать повторную регистрацию:

```tsx
const handler = useCallback(
  (notification: Notification) => alerts.show(notification.text),
  [alerts]
);
useSignalRHandler('Namespace.Messages.Notification', handler);
```

Для глобального обработчика доступен `useSignalRGlobalHandler()`. Хук
регистрирует его через `onGlobal()` и автоматически снимает регистрацию при
размонтировании или замене модуля:

```tsx
useSignalRGlobalHandler((payload, context) => {
  console.log(context.type, payload);
});
```

Так как реестр хранит только один глобальный обработчик, несколько одновременных
вызовов `useSignalRGlobalHandler()` не объединяются: активным остаётся последний
зарегистрированный hook.

## Ошибки и ограничения

- Ошибки соединения передаются в `onError`.
- Ошибки пользовательских обработчиков не прерывают доставку других сообщений и передаются в `onCallbackError`.
- Пакет не делает runtime-валидацию payload по типам TypeScript.
- Backend платформы принимает и отправляет через SignalR только `IDomainEvent` и `IRemoteEvent`.
- По умолчанию входящие события backend платформы передаёт через транспортный метод `ReceiveMessage`.
