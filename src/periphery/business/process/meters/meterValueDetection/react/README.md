# Meter value detection for React Native

Самодостаточный Expo-модуль распознавания показаний счётчиков. Модуль не подключён к шаблону приложения автоматически.

Для подключения добавьте workspace-пакет `@tmobis/business-process-meter-value-detection` в зависимости Expo-приложения и выполните native rebuild. Expo autolinking обнаружит `expo-module.config.json` через зависимость приложения.

Публичный API экспортируется из `index.ts`, включая `MeterValueDetectionCamera`, сервис распознавания и низкоуровневый нативный вызов.
