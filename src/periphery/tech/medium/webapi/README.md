# HTTP-контракт EntityEndpointBuilder

REST-маршруты сущностей:

| Операция | Метод | URL |
| --- | --- | --- |
| Создание | POST | `/api/{Entity}` |
| Обновление | PUT | `/api/{Entity}/{id}` |
| Получение | GET | `/api/{Entity}/{id}` |
| Удаление | DELETE | `/api/{Entity}/{id}` |

ID при обновлении берётся **из URL**; если в JSON body передан другой `id`, сервер перезапишет его значением из URL.
При обновлении отсутствующей сущности возвращается `404 Not Found`, при `Guid.Empty` — `400 Bad Request`.
Создание возвращает `201 Created` с JSON-сущностью и заголовком `Location: /api/{Entity}/{id}`.
Обновление возвращает `200 OK` и JSON-сущность.

В React API `updateEntity(data, entityType)` поддерживается, если `data.id` присутствует.
Для данных без ID в теле используйте `updateEntity(data, entityType, id)`.
Стандартная форма редактирования берёт ID из маршрута страницы.
