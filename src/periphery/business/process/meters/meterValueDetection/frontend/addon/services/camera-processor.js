import { Promise } from 'rsvp';
import Service from '@ember/service';

/**
  @class CameraProcessorService
  @description Сервис управления нативной камерой через cordova-plugin-camera-preview.
    Отвечает за запуск/остановку камеры, управление вспышкой и захват кадров
    (как для финального снимка, так и для live-OCR).
*/
export default Service.extend({
  /**
    @property isCameraActive
    @type {Boolean}
    @description Активна ли сейчас камера.
  */
  isCameraActive: false,

  /**
    @property isFlashOn
    @type {Boolean}
    @description Включена ли сейчас вспышка (режим torch).
  */
  isFlashOn: false,

  /**
    @property overlayFrameCoords
    @description Свойство для обновления координат оверлея камеры при перемещении.
  */
  overlayFrameCoords: null,

  /**
    @method startCamera
    @param {Object} [options] Опции, перекрывающие настройки по умолчанию.
    @return {RSVP.Promise}
    @description Запускает нативную камеру. Преview позиционируется по DOM-элементу
      `.camera-target-frame`, если он отрисован в момент запуска, иначе — на весь экран
      минус 200px снизу под панель управления.
  */
  startCamera(options) {
    if (this.get('isCameraActive')) {
      return Promise.resolve();
    }

    options = options || {};

    return new Promise((resolve, reject) => {
      document.addEventListener('deviceready', () => {
        if (!this.isPluginAvailable()) {
          reject(new Error('Camera plugin not available'));
          return;
        }

        // Небольшая задержка, чтобы ember успел отрендерить .camera-target-frame
        setTimeout(() => {
          this.doStartCamera(options, resolve, reject);
        }, 100);
      });
    });
  },

  /**
    @method stopCamera
    @return {RSVP.Promise}
    @description Останавливает камеру и сбрасывает состояние.
  */
  stopCamera() {
    if (!this.get('isCameraActive')) {
      return Promise.resolve();
    }

    return new Promise((resolve) => {
      const finalize = () => {
        this.set('isCameraActive', false);
        this.set('isFlashOn', false);
        this.sendFlashState();
        resolve();
      };

      CameraPreview.stopCamera(finalize, (error) => {
        console.error('[CameraProcessor] stopCamera error:', error);
        finalize();
      });
    });
  },

  /**
    @method toggleFlash
    @return {RSVP.Promise}
    @description Переключает вспышку между off и torch. Если torch не поддерживается,
      пробует режим on. В случае неудачи уведомляет пользователя.
  */
  toggleFlash() {
    if (!this.get('isCameraActive')) {
      return Promise.resolve();
    }

    const newState = !this.get('isFlashOn');
    const primaryMode = newState ? 'torch' : 'off';
    const fallbackMode = newState ? 'on' : 'off';

    return new Promise((resolve) => {
      const onSuccess = () => {
        this.set('isFlashOn', newState);
        this.sendFlashState();
        resolve();
      };

      CameraPreview.setFlashMode(primaryMode, onSuccess, () => {
        CameraPreview.setFlashMode(fallbackMode, onSuccess, (err) => {
          console.error('[CameraProcessor] Flash not supported:', err);
          alert('Вспышка не поддерживается на этом устройстве');
          resolve();
        });
      });
    });
  },

  /**
    @method captureFrame
    @return {RSVP.Promise<{full: HTMLCanvasElement, cropped: HTMLCanvasElement}>}
    @description Делает полноценное фото высокого качества.
      Возвращает два canvas: полный и обрезанный по области рамки-наводки.
  */
  captureFrame() {
    return new Promise((resolve, reject) => {
      const options = { quality: 85, width: 1280, height: 960 };

      CameraPreview.takePicture(options, (imageData) => {
        if (!imageData) {
          reject(new Error('No image data'));
          return;
        }
        this.base64ToCanvas(imageData)
          .then((fullCanvas) => {
            resolve({
              full: fullCanvas,
              cropped: this.cropToOverlayFrame(fullCanvas)
            });
          })
          .catch(reject);
      }, reject);
    });
  },

  /**
    @method captureFrameForOCR
    @return {RSVP.Promise<HTMLCanvasElement>}
    @description Быстрый снимок для live-OCR: уменьшает превью до 720px
      по высоте и обрезает по области рамки-наводки. Без тяжёлой обработки —
      препроцессинг выполняется внутри нативной tflite-модели.
  */
  captureFrameForOCR() {
    return new Promise((resolve, reject) => {
      if (!this.get('isCameraActive')) {
        reject(new Error('Camera not active'));
        return;
      }

      CameraPreview.takeSnapshot({ quality: 85 }, (imageData) => {
        if (!imageData) {
          reject(new Error('No snapshot data'));
          return;
        }
        this.base64ToCanvas(imageData)
          .then((canvas) => {
            const downscaled = this.downscaleCanvas(canvas, 720);
            resolve(this.cropToOverlayFrame(downscaled));
          })
          .catch(reject);
      }, reject);
    });
  },

  refreshFrameCoords() {
    if (!this.get('isCameraActive')) return Promise.resolve();
  
    return new Promise((resolve) => {
        CameraPreview.getFrameRect(
            (coords) => {
                this.set('overlayFrameCoords', coords);
                resolve();
            },
            () => resolve()
        );
    });
  },

  doStartCamera(options, resolve, reject) {
    const targetFrame = document.querySelector('.camera-target-frame');
    let rect;

    if (targetFrame) {
      const r = targetFrame.getBoundingClientRect();
      rect = {
        x: Math.max(0, r.left),
        y: Math.max(0, r.top),
        width: Math.min(r.width, window.screen.width - r.left),
        height: Math.min(r.height, window.screen.height - r.top)
      };
    } else {
      rect = { x: 0, y: 0, width: window.screen.width, height: window.screen.height - 200 };
    }

    const defaults = {
      x: rect.x,
      y: rect.y,
      width: rect.width,
      height: rect.height,
      camera: CameraPreview.CAMERA_DIRECTION.BACK,
      tapPhoto: false,
      tapFocus: true,
      previewDrag: false,
      toBack: false,
      alpha: 1
    };

    CameraPreview.startCamera(Object.assign({}, defaults, options), () => {
      this.set('isCameraActive', true);
      this.set('isFlashOn', false);
      this.sendFlashState();
      resolve();
    }, (error) => {
      console.error('[CameraProcessor] startCamera error:', error);
      reject(error);
    });
  },

  cropToOverlayFrame(sourceCanvas) {
    const coords = this.get('overlayFrameCoords');
    const relLeft = coords ? coords[0] : 0.1;
    const relTop = coords ? coords[1] : 0.35;
    const relWidth = coords ? coords[2] : 0.8;
    const relHeight = coords ? coords[3] : 0.3;
  
    const frameX = Math.round(sourceCanvas.width * relLeft);
    const frameY = Math.round(sourceCanvas.height * relTop);
    const frameW = Math.round(sourceCanvas.width * relWidth);
    const frameH = Math.round(sourceCanvas.height * relHeight);
  
    const safeX = Math.max(0, Math.min(frameX, sourceCanvas.width - 1));
    const safeY = Math.max(0, Math.min(frameY, sourceCanvas.height - 1));
    const safeW = Math.min(frameW, sourceCanvas.width - safeX);
    const safeH = Math.min(frameH, sourceCanvas.height - safeY);
  
    if (safeW <= 0 || safeH <= 0) {
        return sourceCanvas;
    }
  
    const cropped = document.createElement('canvas');
    cropped.width = safeW;
    cropped.height = safeH;
    cropped.getContext('2d').drawImage(
        sourceCanvas,
        safeX, safeY, safeW, safeH,
        0, 0, safeW, safeH
    );
    return cropped;
  },

  downscaleCanvas(sourceCanvas, maxHeight) {
    if (sourceCanvas.height <= maxHeight) {
      return sourceCanvas;
    }
    const scale = maxHeight / sourceCanvas.height;
    const w = Math.round(sourceCanvas.width * scale);
    const h = Math.round(sourceCanvas.height * scale);

    const dst = document.createElement('canvas');
    dst.width = w;
    dst.height = h;

    const ctx = dst.getContext('2d');
    ctx.imageSmoothingEnabled = true;
    ctx.imageSmoothingQuality = 'high';
    ctx.drawImage(sourceCanvas, 0, 0, w, h);
    return dst;
  },

  base64ToCanvas(base64Data) {
    return new Promise((resolve, reject) => {
      const img = new Image();
      img.onload = () => {
        const canvas = document.createElement('canvas');
        canvas.width = img.width;
        canvas.height = img.height;
        canvas.getContext('2d').drawImage(img, 0, 0);
        resolve(canvas);
      };
      img.onerror = () => reject(new Error('Failed to load image'));
      img.src = `data:image/jpeg;base64,${base64Data}`;
    });
  },

  sendFlashState() {
    window.dispatchEvent(new CustomEvent('flashStateChanged', {
      detail: { isFlashOn: this.get('isFlashOn') }
    }));
  },

  isPluginAvailable() {
    return typeof window.CameraPreview !== 'undefined';
  },

  willDestroy() {
    this.stopCamera();
    this._super(...arguments);
  }
});