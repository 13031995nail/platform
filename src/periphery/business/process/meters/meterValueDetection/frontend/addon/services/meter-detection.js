import { Promise } from 'rsvp';
import Service from '@ember/service';

/**
  @class MeterDetectionService
  @description Сервис распознавания показаний счётчика через нативный плагин
    YOLOv5MeterRecognition. Поддерживает как одиночное распознавание, так и непрерывный режим
    с автоматическим усреднением результатов.
*/
export default Service.extend({
  /**
    @property continuousResults
    @type {Array<Object>}
    @description Буфер последних результатов непрерывного распознавания. Управляется startContinuousDetection/stopContinuousDetection.
  */
  continuousResults: undefined,

  /**
    @property continuousInterval
    @type {Number}
    @description Идентификатор setInterval активного цикла распознавания.
  */
  continuousInterval: null,

  /**
    @property isRunning
    @type {Boolean}
    @description Флаг: активен ли сейчас непрерывный режим распознавания.
  */
  isRunning: false,

  init() {
    this._super(...arguments);
    this.set('continuousResults', []);
  },

  /**
    @method initialize
    @return {RSVP.Promise}
    @description Инициализация сервиса. Оставлена для совместимости
      со старым API (tesseract требовал асинхронной загрузки worker'а).
      Нативная YOLO-модель загружается плагином при старте приложения.
  */
  initialize() {
    return Promise.resolve();
  },

  /**
    @method detect
    @param {HTMLCanvasElement|String} element Canvas или base64-строка
      (в т.ч. data URL) с изображением для распознавания.
    @return {RSVP.Promise<Object>} Результат: { text, wholePart, fractionalPart, hasDot, confidence }.
    @description Распознаёт показания на изображении через нативный YOLOv5-плагин.
  */
  detect(element) {
    return new Promise((resolve, reject) => {
      const base64Image = this.elementToBase64(element);

      cordova.exec(
        (result) => resolve(result),
        (error) => reject(new Error(error)),
        'YOLOv5MeterRecognition',
        'detectDigits',
        [base64Image]
      );
    });
  },

  /**
    @method startContinuousDetection
    @param {Function} getFrameCallback Функция, возвращающая кадр (canvas или Promise<canvas>).
    @param {Function} onResult Вызывается на каждом успешном кадре
      с объектом { text, wholePart, fractionalPart, hasDot, confidence, allResults, isReliable }.
      Поле confidence — уверенность с учётом стабильности результата между кадрами.
    @param {Object} [options]
    @param {Number} [options.intervalMs=1500] Интервал между кадрами в мс.
    @param {Number} [options.maxResults=5] Размер буфера результатов.
    @param {Number} [options.minLength=0] Минимальная длина целой части показания
      для попадания в буфер (0 — без ограничения).
    @description Запускает цикл непрерывного распознавания кадров.
      Если уже запущен — перезапускается.
  */
  startContinuousDetection(getFrameCallback, onResult, options) {
    if (this.get('isRunning')) {
      this.stopContinuousDetection();
    }

    options = options || {};
    const intervalMs = options.intervalMs || 1500;
    const maxResults = options.maxResults || 5;
    const minLength = options.minLength || 0;

    this.set('continuousResults', []);
    this.set('isRunning', true);

    let processingFrame = false;

    const interval = setInterval(() => {
      if (!this.get('isRunning') || processingFrame) {
        return;
      }
      processingFrame = true;

      const done = () => { processingFrame = false; };
      const framePromise = getFrameCallback();

      if (framePromise && typeof framePromise.then === 'function') {
        framePromise
          .then((frame) => this.processFrame(frame, maxResults, minLength, onResult, done))
          .catch((err) => {
            console.error('[MeterDetection] Frame error:', err);
            done();
          });
      } else {
        this.processFrame(framePromise, maxResults, minLength, onResult, done);
      }
    }, intervalMs);

    this.set('continuousInterval', interval);
  },

  /**
    @method stopContinuousDetection
    @description Останавливает непрерывное распознавание и очищает буфер.
  */
  stopContinuousDetection() {
    const interval = this.get('continuousInterval');
    if (interval) {
      clearInterval(interval);
      this.set('continuousInterval', null);
    }
    this.set('isRunning', false);
    this.set('continuousResults', []);
  },

  /**
    @method elementToBase64
    @param {HTMLCanvasElement|String} element
    @return {String} Чистая base64-строка без префикса data:image/*;base64,.
  */
  elementToBase64(element) {
    if (element instanceof HTMLCanvasElement) {
      return element.toDataURL('image/jpeg', 0.9).split(',')[1];
    }
    if (typeof element === 'string') {
      return element.replace(/^data:image\/[a-z]+;base64,/, '');
    }
    return '';
  },

  /**
    @method processFrame
    @description Обрабатывает один кадр: распознаёт, кладёт в буфер,
      считает наиболее вероятный результат, его стабильность и итоговую уверенность.
  */
  processFrame(frame, maxResults, minLength, onResult, callback) {
    if (!frame) {
      callback();
      return;
    }

    this.detect(frame)
      .then((result) => {
        const text = (result.text || '').trim();
        const wholePart = result.wholePart || '';

        // Пустой результат — модель ничего не увидела, в буфер не пишем.
        if (text === '' && wholePart === '') {
          callback();
          return;
        }

        // Проверка минимальной длины целой части (если задана).
        if (minLength > 0 && wholePart.length < minLength) {
          callback();
          return;
        }

        const results = this.get('continuousResults');
        results.push({
          text,
          wholePart,
          fractionalPart: result.fractionalPart,
          hasDot: result.hasDot,
          modelConfidence: result.confidence || 0,
          timestamp: Date.now()
        });
        if (results.length > maxResults) {
          results.shift();
        }

        const best = this.getBestResult(results);
        const matching = results.filter((r) => r.text === best.text);
        const stability = results.length > 1 ? matching.length / results.length : 1;

        // Итоговая уверенность: средняя уверенность модели по совпадающим кадрам,
        // умноженная на стабильность. При одном кадре в буфере — чистая уверенность модели.
        
        const avgModelConfidence = matching.reduce((sum, r) => sum + r.modelConfidence, 0) / matching.length;
        const displayConfidence = Math.round(avgModelConfidence * stability);

        const isReliable = matching.length >= 2;

        if (typeof onResult === 'function') {
          onResult({
            text: best.text,
            wholePart: best.wholePart,
            fractionalPart: best.fractionalPart,
            hasDot: best.hasDot,
            confidence: displayConfidence,
            allResults: results.slice(),
            isReliable
          });
        }

        callback();
      })
      .catch((err) => {
        console.error('[MeterDetection] Detect error:', err);
        callback();
      });
  },

  /**
    @method getBestResult
    @description Выбирает наиболее частый результат из буфера.
  */
  getBestResult(results) {
    if (!results || results.length === 0) {
      return { text: '', modelConfidence: 0 };
    }
    if (results.length === 1) {
      return results[0];
    }

    const groups = {};
    results.forEach((r) => {
      if (!groups[r.text]) {
        groups[r.text] = Object.assign({}, r, { count: 0 });
      }
      groups[r.text].count += 1;
    });

    let best = null;
    Object.keys(groups).forEach((key) => {
      const group = groups[key];
      if (!best || group.count > best.count) {
        best = group;
      }
    });

    return best;
  },

  willDestroy() {
    this._super(...arguments);
    this.stopContinuousDetection();
  }
});