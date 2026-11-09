import { inject as service } from '@ember/service';
import Mixin from '@ember/object/mixin';
import { isNone } from '@ember/utils';

/**
  @class MeterDetectionComponentMixin
  @description Миксин для компонентов, работающих с распознаванием показаний
    счётчика по фотографии. Подключает сервис распознавания и даёт action
    `detectRate` для ручного запуска распознавания по элементу `#ratePhoto`.
*/
export default Mixin.create({
  /**
    @property meterDetection
    @type {Service}
    @description Сервис распознавания показаний (YOLOv5).
  */
  meterDetection: service('meter-detection'),

  /**
    @property meterDetectionInitialized
    @type {Boolean}
    @description Флаг готовности сервиса распознавания.
  */
  meterDetectionInitialized: false,

  /**
    @property meterDetectionInitialization
    @type {RSVP.Promise}
    @description Промис, разрешающийся после инициализации сервиса распознавания.
  */
  meterDetectionInitialization: undefined,

  /**
    @property isDetecting
    @type {Boolean}
    @description Идёт ли сейчас процесс распознавания.
  */
  isDetecting: false,

  /**
    @property recognizedValue
    @type {String|Number}
    @description Результат распознавания - значение показания счётчика.
  */
  recognizedValue: undefined,

  /**
    @property recognitionConfidence
    @type {Number}
    @description Уверенность распознавания в процентах (0-100).
  */
  recognitionConfidence: undefined,

  init() {
    this._super(...arguments);

    const initialization = this.get('meterDetection').initialize().then(() => {
      this.set('meterDetectionInitialized', true);
    });
    this.set('meterDetectionInitialization', initialization);
  },

  /**
    @method showDetectionLoading
    @param {String} message Текст индикатора.
    @description Сообщает хосту о старте распознавания через опциональный хук
      `onDetectStart(message)` (если он передан). По умолчанию - no-op: модуль не
      навязывает свой индикатор загрузки, это ответственность проекта-хоста.
  */
  showDetectionLoading(message) {
    const hook = this.get('onDetectStart');
    if (typeof hook === 'function') {
      hook(message);
    }
  },

  /**
    @method hideDetectionLoading
    @description Сообщает хосту о завершении распознавания через опциональный хук
      `onDetectEnd()` (если он передан). По умолчанию - no-op.
  */
  hideDetectionLoading() {
    const hook = this.get('onDetectEnd');
    if (typeof hook === 'function') {
      hook();
    }
  },

  /**
    @method performDetection
    @return {RSVP.Promise}
    @description Запускает распознавание показаний на элементе `#ratePhoto`.
      Результат записывается в `recognizedValue` и `recognitionConfidence`.
      Возвращает промис - можно дождаться завершения (в отличие от action `detectRate`).
  */
  async performDetection() {
    if (this.get('isDetecting')) {
      return;
    }

    const img = document.getElementById('ratePhoto');
    if (!img || !img.complete) {
      return;
    }

    this.set('isDetecting', true);
    this.showDetectionLoading('Распознавание...');

    try {
      const canvas = document.createElement('canvas');
      canvas.width = img.naturalWidth;
      canvas.height = img.naturalHeight;
      canvas.getContext('2d').drawImage(img, 0, 0);

      const data = await this.get('meterDetection').detect(canvas);
      const text = (data.text || '').trim();
      const numeric = parseFloat(text);

      this.set('recognizedValue', isNaN(numeric) ? '' : numeric);
      this.set('recognitionConfidence', isNone(data.confidence) ? null : Math.round(data.confidence));
    } finally {
      this.set('isDetecting', false);
      this.hideDetectionLoading();
    }
  },

  actions: {
    /**
      @method actions.detectRate
      @description Action-обёртка над `performDetection` для использования из шаблонов.
    */
    detectRate() {
      this.performDetection();
    }
  }
});
