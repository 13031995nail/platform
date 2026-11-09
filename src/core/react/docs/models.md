## Генерация моделей
Генерация моделей для фронтенда выполняется с помощью OpenAPI генератора на основе схемы из бэкенда.

Команда для генерации: 
```
 yarn openapi-generator-cli generate -i http://адресБэкенда/swagger/v1/swagger.json -g typescript-axios -o src/models --additional-properties=withSeparateModelsAndApi=true,apiPackage=apis,modelPackage=. -t node_modules/@tmobis/core/oapi-gen --global-property models
```

Модели складываются в папку `src/models`.

Шаблон генерации (`modelGeneric.mustache`) расположен в `@tmobis/core/oapi-gen/` и подключается через `-t node_modules/@tmobis/core/oapi-gen`.