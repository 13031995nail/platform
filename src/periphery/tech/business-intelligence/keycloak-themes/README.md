# Изменение темы формы авторизации в Keycloak

Инструкция описывает, как создать кастомную тему входа с собственными стилями и логотипом, смонтировать её в контейнер Keycloak и активировать в панели управления.

> 💡 **Примечание:** В примерах ниже используется название темы `krasnoyarsk`. Замените его на имя своей темы (оно будет отображаться в настройках Keycloak).

---

## 1. Создание структуры каталогов

Создайте каталог `themes` **в той же папке, где находится файл `docker-compose.yml`**. Внутри него подготовьте следующую структуру:

```text
каталог-проекта/
├── docker-compose.yml
└── themes/
    └── krasnoyarsk/
        └── login/
            ├── theme.properties
            └── resources/
                ├── css/
                │   └── login.css
                └── img/
                    └── logo.svg
```

## 2. Настройка свойств темы

Внесите конфигурацию базовой темы в файл `themes/krasnoyarsk/login/theme.properties`:

```properties
parent=keycloak
import=common/keycloak
styles=css/login.css
```

## 3. Добавление CSS и логотипа

1. Добавьте кастомные стили в файл `themes/krasnoyarsk/login/resources/css/login.css`. 
   
   *Пример подключения логотипа над формой:*
   ```css
   .login-pf-page .card-pf::before {
     content: "";
     display: block;
     height: 76px;
     margin: 4px 0 18px;
     background: url("../img/logo.svg") no-repeat center / contain;
   }
   ```

2. Поместите ваш SVG-логотип в каталог: `themes/krasnoyarsk/login/resources/img/logo.svg`

## 4. Подключение темы в Docker Compose

Примонтируйте созданную папку с темой в контейнер Keycloak через секцию `volumes` в `docker-compose.yml`:

```yaml

services:
  keycloak:
    # ... остальные настройки сервиса ...
    volumes:
      - ./themes/krasnoyarsk:/opt/keycloak/themes/krasnoyarsk:ro
```

## 5. Включение темы в панели Keycloak

1. Откройте **Admin Console** вашего Keycloak.
2. Перейдите в левом меню в раздел **Realm settings** и выберите вкладку **Themes**.
3. В выпадающем списке **Login Theme** выберите вашу тему (`krasnoyarsk`).
4. Нажмите кнопку **Save**.
5. Выйдите из аккаунта или откройте инкогнито-вкладку, чтобы проверить изменения на странице входа.
