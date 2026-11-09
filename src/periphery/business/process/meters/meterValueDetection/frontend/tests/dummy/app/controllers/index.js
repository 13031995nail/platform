import Controller from '@ember/controller';

/**
  Контроллер формы-примера распознавания показаний.
  Получает результат от компонента `meter-value-detection-camera` и
  сохраняет его для отображения (в реальном приложении здесь была бы
  запись фото и значения в модель/хранилище).
*/
export default Controller.extend({
  savedValue: null,
  savedConfidence: null,
  savedImage: null,

  actions: {
    /**
      Колбэк результата распознавания.
      @param {String|Number} value Распознанное значение.
      @param {Number} confidence Уверенность, %.
      @param {String} imageDataUrl Снимок (data URL).
    */
    onMeterRecognized(value, confidence, imageDataUrl) {
      this.setProperties({
        savedValue: value,
        savedConfidence: confidence,
        savedImage: imageDataUrl
      });
    }
  }
});
