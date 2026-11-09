import Component from '@ember/component';
import { inject as service } from '@ember/service';
import layout from '../templates/components/meter-value-detection-camera';
import MeterDetectionComponent from '../mixins/meter-detection-component';

/**
  @class MeterValueDetectionCameraComponent
  @description Переиспользуемый компонент распознавания показаний приборов учёта
    на устройстве (YOLOv5 TFLite). Включает в себя:
      - открытие/закрытие нативной камеры (cordova-plugin-camera-preview);
      - непрерывное (live) распознавание кадров с подсветкой уверенности;
      - захват кадра и ручное распознавание по снимку.

    Не зависит от модели файлов/хранилища конкретного проекта. Результат
    отдаётся наружу через действие `onResult(value, confidence, imageDataUrl)`.

    Пример использования:
      {{meter-value-detection-camera onResult=(action "onMeterRecognized")}}
*/
export default Component.extend(MeterDetectionComponent, {
  layout,

  tagName: '',

  /**
    @property cameraProcessor
    @type {Service}
    @description Сервис управления нативной камерой.
  */
  cameraProcessor: service(),

  /**
    @property buttonCaption
    @type {String}
    @description Подпись кнопки открытия камеры.
  */
  buttonCaption: 'Распознать показания',

  /**
    @property readonly
    @type {Boolean}
    @description Только просмотр - скрывает кнопку распознавания.
  */
  readonly: false,

  /**
    @property hidePreview
    @type {Boolean}
    @description Режим «предпросмотр на стороне хоста»: после съёмки компонент не показывает
      собственный предпросмотр снимка, а сразу отдаёт результат через `onResult` и возвращается
      к кнопке. Можно использовать, когда у формы уже есть своё окно предпросмотра/сохранения.
  */
  hidePreview: false,

  /** Флаг показа полноэкранной камеры. */
  showCamera: false,

  /** Состояние вспышки (синхронизируется с сервисом камеры). */
  cameraFlashOn: false,

  /** Последнее распознанное значение в live-режиме. */
  liveValue: '',

  /** Уверенность live-распознавания. */
  liveConfidence: 0,

  /** CSS-класс индикатора уверенности. */
  liveValueClass: '',

  /** Активен ли live-режим. */
  isLiveDetecting: false,

  /** Снимок после захвата кадра - показывается в предпросмотре. */
  capturedImage: null,

  init() {
    this._super(...arguments);

    this.flashStateListener = (event) => {
      this.set('cameraFlashOn', event.detail.isFlashOn);
    };
    window.addEventListener('flashStateChanged', this.flashStateListener);
  },

  willDestroy() {
    this._super(...arguments);
    this.stopLiveDetection();
    if (this.flashStateListener) {
      window.removeEventListener('flashStateChanged', this.flashStateListener);
    }
  },

  /**
    @method startLiveDetection
    @description Запускает непрерывное распознавание кадров с камеры.
  */
  startLiveDetection() {
    const cameraProcessor = this.get('cameraProcessor');

    this.setProperties({
      liveValue: '',
      liveConfidence: 0,
      liveValueClass: '',
      isLiveDetecting: true
    });

    const getFrame = () => {
      if (!cameraProcessor.get('isCameraActive')) {
        return null;
      }
      // Перед каждым кадром обновляем координаты рамки (её можно двигать/растягивать),
      // чтобы live-OCR обрезал именно по текущей рамке.
      return cameraProcessor.refreshFrameCoords()
        .then(() => cameraProcessor.captureFrameForOCR())
        .catch(() => null);
    };

    const onResult = (result) => {
      if (!this.get('showCamera')) {
        return;
      }
      this.setProperties({
        liveValue: result.text,
        liveConfidence: result.confidence,
        liveValueClass: this.confidenceClass(result.confidence, result.isReliable)
      });
    };

    this.get('meterDetection').startContinuousDetection(getFrame, onResult, {
      intervalMs: 1500,
      maxResults: 5
    });
  },

  /**
    @method stopLiveDetection
    @description Останавливает непрерывное распознавание и сбрасывает live-поля.
  */
  stopLiveDetection() {
    this.get('meterDetection').stopContinuousDetection();
    this.setProperties({
      isLiveDetecting: false,
      liveValue: '',
      liveConfidence: 0,
      liveValueClass: ''
    });
  },

  /**
    @method confidenceClass
    @param {Number} confidence Уверенность 0-100.
    @param {Boolean} isReliable Подтверждена ли повторяемость кадров.
    @return {String} CSS-класс индикатора (high/medium/low-confidence).
  */
  confidenceClass(confidence, isReliable) {
    if (!isReliable) {
      return 'low-confidence';
    }
    if (confidence >= 75) {
      return 'high-confidence';
    }
    if (confidence >= 50) {
      return 'medium-confidence';
    }
    return 'low-confidence';
  },

  /**
    @method closeCameraInternal
    @return {Promise}
    @description Закрывает камеру и чистит состояние.
  */
  async closeCameraInternal() {
    this.stopLiveDetection();
    this.set('showCamera', false);
    document.documentElement.classList.remove('camera-active');
    document.body.classList.remove('camera-active');
    try {
      await this.get('cameraProcessor').stopCamera();
    } catch (e) {
      console.error('[MeterValueDetectionCamera] stopCamera error:', e);
    }
  },

  /**
    @method emitResult
    @description Сообщает результат хосту через действие `onResult`, если оно задано.
  */
  emitResult() {
    const onResult = this.get('onResult');
    if (typeof onResult === 'function') {
      onResult(this.get('recognizedValue'), this.get('recognitionConfidence'), this.get('capturedImage'));
    }
  },

  actions: {
    /**
      @method actions.openCamera
      @description Открывает полноэкранный оверлей камеры и запускает live-распознавание.
    */
    async openCamera() {
      try {
        this.set('capturedImage', null);
        this.set('recognizedValue', null);
        this.set('recognitionConfidence', null);
        this.set('showCamera', true);
        document.documentElement.classList.add('camera-active');
        document.body.classList.add('camera-active');

        await this.get('cameraProcessor').startCamera({ toBack: false, alpha: 1 });
        this.set('cameraFlashOn', this.get('cameraProcessor.isFlashOn'));
        this.startLiveDetection();
      } catch (e) {
        console.error('[MeterValueDetectionCamera] openCamera error:', e);
        alert('Ошибка запуска камеры');
        this.closeCameraInternal();
      }
    },

    /**
      @method actions.closeCamera
      @description Закрывает оверлей камеры без съёмки.
    */
    closeCamera() {
      this.closeCameraInternal();
    },

    /**
      @method actions.takePicture
      @description Делает снимок, закрывает камеру и показывает предпросмотр
        с предварительным (live) значением.
    */
    async takePicture() {
      try {
        const cameraProcessor = this.get('cameraProcessor');
        if (!cameraProcessor.get('isCameraActive')) {
          alert('Камера не активна');
          return;
        }

        // Синхронизируем координаты рамки перед финальным снимком,
        // чтобы обрезка совпала с тем, что видит пользователь.
        await cameraProcessor.refreshFrameCoords();

        const { full } = await cameraProcessor.captureFrame();
        const imageData = full.toDataURL('image/jpeg');

        const liveValue = this.get('liveValue');
        const liveConfidence = this.get('liveConfidence');

        await this.closeCameraInternal();

        this.setProperties({
          recognizedValue: liveValue || null,
          recognitionConfidence: liveValue ? liveConfidence : null
        });

        if (this.get('hidePreview')) {
          // Предпросмотр на стороне хоста: свой не показываем, всегда отдаём снимок+значение.
          this.set('capturedImage', null);
          const onResult = this.get('onResult');
          if (typeof onResult === 'function') {
            onResult(this.get('recognizedValue'), this.get('recognitionConfidence'), imageData);
          }
        } else {
          this.set('capturedImage', imageData);
          if (liveValue) {
            this.emitResult();
          }
        }
      } catch (e) {
        console.error('[MeterValueDetectionCamera] takePicture error:', e);
        alert('Ошибка при съёмке фото: ' + (e.message || 'Неизвестная ошибка'));
      }
    },

    /**
      @method actions.toggleFlash
      @description Переключает вспышку камеры.
    */
    async toggleFlash() {
      await this.get('cameraProcessor').toggleFlash();
    },

    /**
      @method actions.detectFromPhoto
      @description Ручное распознавание по сделанному снимку (элемент `#ratePhoto`).
        После распознавания отдаёт результат хосту.
    */
    async detectFromPhoto() {
      await this.performDetection();
      this.emitResult();
    }
  }
});
