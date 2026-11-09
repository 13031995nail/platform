# Модуль распознавания показаний приборов учёта (meterValueDetection)

Модуль для распознавания показаний приборов учёта (электроэнергия / вода) **на устройстве**: 
> видеопоток камеры -> захват кадра -> нейросеть YOLOv5s в формате TFLite -> распознанное значение

## Состав модуля

```
meterValueDetection/
├── backend/
│   ├── model/
│   └── consumers/
├── frontend/        - Ember-аддон: сервисы, миксин, компонент, стили + форма-пример (tests/dummy)
├── mobile/          - Cordova-плагин: нативный YOLOv5 TFLite-инференс + модель
├── camera-preview/  - форк cordova-plugin-camera-preview с нативной рамкой-наведения
└── example/         - APK-пример: Cordova-обёртка, собирающая frontend + mobile в Android-приложение
```

On-device мобильное распознавание **не обращается** к backend-модулю и работает автономно.

---

## Frontend (Ember-аддон)

Пакет: `@timepath/tmobis-business-process-meters-meter-value-detection`.

Что внутри (`addon/`):
- `services/meter-detection` - обёртка над нативным плагином (одиночное и непрерывное
  (live) распознавание с усреднением и оценкой уверенности);
- `services/camera-processor` - старт/стоп камеры, вспышка, захват кадра, обрезка по рамке наводки;
- `mixins/meter-detection-component` - подключение сервиса + распознавание по элементу `#ratePhoto`;
- `components/meter-value-detection-camera` - готовый переиспользуемый компонент
  (камера + live-OCR + снимок), отдаёт результат через `onResult(value, confidence, imageDataUrl)`;
- `styles/meter-value-detection.scss` - стили оверлея камеры и live-OCR.

### Подключение в приложении

```bash
yarn add @timepath/tmobis-business-process-meters-meter-value-detection
```

Использование компонента:

```hbs
{{meter-value-detection-camera onResult=(action "onMeterRecognized")}}
```

```js
actions: {
  onMeterRecognized(value, confidence, imageDataUrl) {
    // сохранить фото и значение в свою модель
  }
}
```

Стили - добавьте каталог стилей аддона в `includePaths`
(`ember-cli-build.js`):

```js
sassOptions: {
  includePaths: ['node_modules/@timepath/tmobis-business-process-meters-meter-value-detection/addon/styles']
}
```

и импортируйте:

```scss
@import 'meter-value-detection';
```

### Проект-пример

`frontend/tests/dummy` - минимальное Ember-приложение с формой распознавания
(маршрут `index`). Запуск из каталога `frontend/`:

```bash
yarn install
ember serve # либо: ember build для тестовой APK
```

В десктоп-браузере нативный плагин недоступен - отображается верстка формы;
полный цикл (камера + распознавание) работает только в мобильной сборке.

---

## Mobile (Cordova-плагин)

Пакет / id: `@timepath/tmobis-cordova-meter-value-detection`
(`tmobis-cordova-meter-value-detection`)

Содержит:
- `src/android/com/tmobis/mlkit/YOLOv5MeterRecognition.java` - Cordova-плагин,
  сервис `YOLOv5MeterRecognition`, действие `detectDigits(base64)`. Вход модели 416×416
  с препроцессингом
- `www/yolov5_meter.tflite` - модель YOLOv5s (≈14 МБ), кладётся в `assets/www/`;
- gradle-зависимости (через `<framework>`): `org.tensorflow:tensorflow-lite:2.13.0`,
  `androidx.exifinterface:exifinterface:1.2.0`.

### Подключение в Cordova-проекте

```bash
# плагин распознавания
cordova plugin add @timepath/tmobis-cordova-meter-value-detection

# камера в видеопотоке - доработанный cordova-camera-preview с нативной рамкой-наведения
cordova plugin add @timepath/tmobis-cordova-camera-preview
```

> (`camera-preview/src/android/CameraOverlay.java`): прямоугольник
> (по умолчанию 80%×30% по центру), оранжевый `#f2711c`, затемнение вокруг, подсказка про наведение. Рамку можно
> **двигать и растягивать** за углы/края - её относительные координаты отдаются в JS через action
> `getFrameRect` (`CameraActivity.getOverlayFrameCoords` → `CameraPreview`), а `camera-processor`
> перед каждым кадром/снимком вызывает `refreshFrameCoords()` и режет кадр (`cropToOverlayFrame`)
> именно по текущей рамке.  Камера запускается с `toBack:false` (поверх webview).

Вызов из JS (frontend-сервис делает это сам):

```js
cordova.exec(onSuccess, onError, 'YOLOv5MeterRecognition', 'detectDigits', [base64Image]);
```

> Модель грузится из ассета `www/yolov5_meter.tflite` при инициализации плагина.

> **Требования к сборке Android:** `minSdk ≥ 22` (TFLite) и `compileSdk ≥ 33`
> (camera-preview-код использует API 33: `TIRAMISU`,
> `READ_MEDIA_IMAGES/VIDEO`). В config.xml:
> `<preference name="android-compileSdkVersion" value="33" />`. Нужен установленный
> Android SDK Platform 33.

---

## Example - пример (фронт + мобилка)

`example/` - Cordova-проект, который собирает **frontend-аддон** (как веб-форму) и
**mobile-плагин** (нативное распознавание) в единое Android-приложение. Веб-часть - это сборка ember dummy-app из `frontend/`.

Состав:
- `config.xml` - Cordova-приложение, подключает плагин `tmobis-cordova-camera-preview`
  (`spec="../camera-preview"`) и плагин `tmobis-cordova-meter-value-detection` (`spec="../mobile"`);
- `package.json` - cordova/cordova-android + скрипты сборки;
- `www/` - генерируется (ember build), в git не хранится.

> **Важно: `example/` исключён из yarn-workspaces и lerna** Платформы (см. `package.json` →
> `workspaces`, `lerna.json`). Причина: cordova при сборке создаёт `plugins/` и `platforms/`
> с собственными `package.json`, которые иначе подхватываются как workspace-пакеты и ломают
> `yarn install`. Cordova-часть собирается **изолированно через npm**

### Сборка и запуск

```powershell
# 1. Веб-часть - из frontend
cd src\periphery\business\process\meters\meterValueDetection\frontend
$env:CORDOVA = 'true'
yarn ember build --environment production --output-path ..\example\www
Remove-Item Env:\CORDOVA

# 2. Cordova-часть - из example, npm
cd ..\example
npm install
cordova platform add android@11.0.0
cordova build android  # или: cordova run android
```

Что проверять на устройстве: открыть приложение → форма распознавания → «Распознать
показания» → камера с рамкой → live-значение → снимок → значение выводится на форму
(в примере без сохранения значения, для демонстрации склейки модулей).

---

## Интеграция в проект

Распознавание подключается как модуль: **frontend** через npm-пакет
`@timepath/*`, **mobile** через два Cordova-плагина

### Шаг 1. Frontend (Ember)

`ember-app/package.json` → `dependencies`:

```json
"@timepath/tmobis-business-process-meters-meter-value-detection": "x.x.x"
```

Встроить компонент в форму (сервисы `meter-detection`/`camera-processor` и миксин приедут с аддоном):

```hbs
{{meter-value-detection-camera onResult=(action "onMeterRecognized")}}
```

```js
actions: {
  onMeterRecognized(value, confidence, imageDataUrl) {
    // сохранить фото (imageDataUrl) и значение (value) в свою модель
  }
}
```

Стили - добавить каталог стилей аддона в `includePaths` (`ember-cli-build.js`):

```js
sassOptions: { includePaths: [
  'node_modules/@timepath/tmobis-business-process-meters-meter-value-detection/addon/styles'
]}
```
и в `app.scss`: `@import 'meter-value-detection';`

### Шаг 2. Мобильная часть (Cordova, Android)

```bash
cordova plugin add @timepath/tmobis-cordova-camera-preview
cordova plugin add @timepath/tmobis-cordova-meter-value-detection
```

`config.xml` (Android): `minSdk 22`, `compileSdk 33`, `targetSdk 33`, `<engine name="android" spec="^11.0.0"/>`.