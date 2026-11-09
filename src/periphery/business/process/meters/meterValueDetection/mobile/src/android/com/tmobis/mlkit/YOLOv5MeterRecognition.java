package com.tmobis.mlkit;

import android.content.res.AssetFileDescriptor;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Canvas;
import android.graphics.Color;
import android.graphics.RectF;
import android.os.Build;
import android.util.Base64;
import android.util.Log;
import org.apache.cordova.CallbackContext;
import org.apache.cordova.CordovaPlugin;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;
import org.tensorflow.lite.Interpreter;
import java.io.FileInputStream;
import java.io.IOException;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.nio.MappedByteBuffer;
import java.nio.channels.FileChannel;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;

public class YOLOv5MeterRecognition extends CordovaPlugin {
    private static final String TAG = "YOLOv5Meter";
    private Interpreter tflite;
    private static final int INPUT_SIZE = 416;
    private static final int NUM_CLASSES = 12;
    private static final float CONF_THRESHOLD = 0.25f;
    private static final float IOU_THRESHOLD = 0.45f;

    // Параметры letterbox-преобразования последнего кадра.
    // Нужны для раскодирования координат боксов обратно в координаты исходной картинки.
    private float lastLetterboxScale = 1.0f;
    private float lastLetterboxPadX = 0f;
    private float lastLetterboxPadY = 0f;

    @Override
    protected void pluginInitialize() {
        super.pluginInitialize();
        try {
            Interpreter.Options options = new Interpreter.Options();
            options.setNumThreads(4);
            tflite = new Interpreter(loadModelFile(), options);
            Log.d(TAG, "YOLOv5 model loaded successfully");
        } catch (Exception e) {
            Log.e(TAG, "Failed to load YOLOv5 model", e);
        }
    }

    @Override
    public boolean execute(String action, JSONArray args, CallbackContext callbackContext) throws JSONException {
        if ("detectDigits".equals(action)) {
            String base64Image = args.getString(0);
            this.detectDigits(base64Image, callbackContext);
            return true;
        }
        return false;
    }

    private void detectDigits(String base64Image, final CallbackContext callbackContext) {
        cordova.getThreadPool().execute(() -> {
            try {
                if (tflite == null) {
                    callbackContext.error("Model not loaded");
                    return;
                }

                byte[] decodedString = Base64.decode(base64Image, Base64.DEFAULT);
                Bitmap bitmap = BitmapFactory.decodeByteArray(decodedString, 0, decodedString.length);

                if (bitmap == null) {
                    callbackContext.error("Failed to decode image");
                    return;
                }

                Log.d(TAG, "Original: " + bitmap.getWidth() + "x" + bitmap.getHeight());

                // preprocessImage применяет letterbox
                ByteBuffer inputBuffer = preprocessImage(bitmap);

                float[][][] output = new float[1][10647][17];

                Log.d(TAG, "Running inference...");
                tflite.run(inputBuffer, output);
                Log.d(TAG, "Inference complete!");

                List<Detection> detections = parseOutput(output[0]);
                List<Detection> filtered = nms(detections);

                // Сортируем слева направо
                Collections.sort(filtered, new Comparator<Detection>() {
                    @Override
                    public int compare(Detection a, Detection b) {
                        return Float.compare(a.box.left, b.box.left);
                    }
                });

                // Разделяем на точку и цифры
                Detection dotDetection = null;
                List<Detection> digitDetections = new ArrayList<>();

                for (Detection d : filtered) {
                    if (d.classId == 11) {
                        dotDetection = d;
                        Log.d(TAG, "  Found dot at x=" + (int)d.box.centerX());
                    } else if (d.classId >= 0 && d.classId <= 9) {
                        digitDetections.add(d);
                    }
                }

                // Фильтрация красных цифр - ТОЛЬКО с правого края.
                // На аналоговых счётчиках дробная часть (последние 1-3 цифры)
                // окрашена в красный. На LCD-счётчиках красных нет вообще.
                // Проверяем цифры справа налево, пока они "красные".
                // Как только встретилась не-красная - все левее неё тоже не красные.
                List<Detection> validDigits = new ArrayList<>(digitDetections);

                if (!digitDetections.isEmpty()) {
                    int removeFromIndex = digitDetections.size();
                    for (int i = digitDetections.size() - 1; i >= 0; i--) {
                        Detection digit = digitDetections.get(i);
                        boolean isRed = isRedDigit(bitmap, digit.box);

                        Log.d(TAG, "  Digit " + digit.classId +
                                " at [" + (int)digit.box.left + "," + (int)digit.box.top + "]" +
                                " isRed=" + isRed);

                        if (isRed) {
                            removeFromIndex = i;
                        } else {
                            break;
                        }
                    }

                    if (removeFromIndex < digitDetections.size()) {
                        List<Detection> redDigits = new ArrayList<>(
                                digitDetections.subList(removeFromIndex, digitDetections.size())
                        );
                        validDigits = new ArrayList<>(
                                digitDetections.subList(0, removeFromIndex)
                        );

                        for (Detection rd : redDigits) {
                            Log.d(TAG, "  REMOVED red tail digit: " + rd.classId);
                        }
                    }
                }

                Log.d(TAG, "Valid digits: " + validDigits.size() +
                        " (removed " + (digitDetections.size() - validDigits.size()) + " red tail digits)");

                // Собираем результат
                StringBuilder wholePart = new StringBuilder();
                StringBuilder fractionalPart = new StringBuilder();

                if (dotDetection != null) {
                    float dotX = dotDetection.box.centerX();

                    for (Detection d : validDigits) {
                        if (d.box.centerX() < dotX) {
                            wholePart.append(d.classId);
                        } else {
                            fractionalPart.append(d.classId);
                        }
                    }
                } else {
                    for (Detection d : validDigits) {
                        wholePart.append(d.classId);
                    }
                }

                // Обработка ведущих нулей
                String wholePartStr = wholePart.toString();
                String fractionalPartStr = fractionalPart.toString();

                if (wholePartStr.length() > 1) {
                    wholePartStr = wholePartStr.replaceFirst("^0+(?!$)", "");
                    if (wholePartStr.isEmpty()) {
                        wholePartStr = "0";
                    }
                }

                // Формируем финальное значение
                String meterValue;
                if (dotDetection != null && fractionalPartStr.length() > 0) {
                    meterValue = wholePartStr + "." + fractionalPartStr;
                } else if (dotDetection != null) {
                    meterValue = wholePartStr + ".";
                } else {
                    meterValue = wholePartStr;
                }

                int avgConfidence = 0;
                if (!validDigits.isEmpty()) {
                    double sum = 0;
                    for (Detection d : validDigits) {
                        sum += d.confidence;
                    }
                    avgConfidence = (int) ((sum / validDigits.size()) * 100);
                }

                JSONObject jsonResult = new JSONObject();
                jsonResult.put("text", meterValue);
                jsonResult.put("wholePart", wholePartStr);
                jsonResult.put("fractionalPart", fractionalPartStr);
                jsonResult.put("hasDot", dotDetection != null);
                jsonResult.put("confidence", avgConfidence);
                jsonResult.put("digitCount", validDigits.size());

                Log.d(TAG, "Detected: '" + meterValue + "' (" + validDigits.size() + " digits)");
                callbackContext.success(jsonResult);

            } catch (Exception e) {
                Log.e(TAG, "Detection error", e);
                callbackContext.error("Error: " + e.getMessage());
            }
        });
    }

    /**
     * Letterbox-преобразование: вписывает картинку в INPUT_SIZE×INPUT_SIZE
     * с сохранением пропорций, заполняя поля серым цветом.
     * Сохраняет scale и padding в поля класса для последующего раскодирования координат.
     */
    private Bitmap letterbox(Bitmap source) {
        int srcW = source.getWidth();
        int srcH = source.getHeight();

        float scale = Math.min((float) INPUT_SIZE / srcW, (float) INPUT_SIZE / srcH);
        int newW = Math.round(srcW * scale);
        int newH = Math.round(srcH * scale);

        int padX = (INPUT_SIZE - newW) / 2;
        int padY = (INPUT_SIZE - newH) / 2;

        // Сохраняем параметры для parseOutput
        lastLetterboxScale = scale;
        lastLetterboxPadX = padX;
        lastLetterboxPadY = padY;

        Bitmap result = Bitmap.createBitmap(INPUT_SIZE, INPUT_SIZE, Bitmap.Config.ARGB_8888);
        Canvas canvas = new Canvas(result);
        canvas.drawColor(Color.rgb(114, 114, 114));

        Bitmap scaled = Bitmap.createScaledBitmap(source, newW, newH, true);
        canvas.drawBitmap(scaled, padX, padY, null);

        if (scaled != source) {
            scaled.recycle();
        }

        Log.d(TAG, "Letterbox: " + srcW + "x" + srcH + " -> " + newW + "x" + newH +
                " (scale=" + String.format("%.3f", scale) +
                ", pad=" + padX + "," + padY + ")");

        return result;
    }

    private ByteBuffer preprocessImage(Bitmap bitmap) {
        // Применяем letterbox всегда (даже если картинка уже квадратная - для единообразия)
        Bitmap letterboxed = letterbox(bitmap);

        ByteBuffer buffer = ByteBuffer.allocateDirect(4 * INPUT_SIZE * INPUT_SIZE * 3);
        buffer.order(ByteOrder.nativeOrder());

        int[] pixels = new int[INPUT_SIZE * INPUT_SIZE];
        letterboxed.getPixels(pixels, 0, INPUT_SIZE, 0, 0, INPUT_SIZE, INPUT_SIZE);

        for (int y = 0; y < INPUT_SIZE; y++) {
            for (int x = 0; x < INPUT_SIZE; x++) {
                int pixel = pixels[y * INPUT_SIZE + x];

                int r = (pixel >> 16) & 0xff;
                int g = (pixel >> 8) & 0xff;
                int b = pixel & 0xff;

                buffer.putFloat(r / 255.0f);
                buffer.putFloat(g / 255.0f);
                buffer.putFloat(b / 255.0f);
            }
        }

        return buffer;
    }

    /**
     * Парсинг output модели с раскодированием координат из letterbox-пространства
     * обратно в координаты исходной картинки.
     */
    private List<Detection> parseOutput(float[][] output) {
        List<Detection> detections = new ArrayList<>();

        int[] classHits = new int[NUM_CLASSES];

        for (int i = 0; i < output.length; i++) {
            float[] detection = output[i];
            float confidence = detection[4];

            if (confidence < CONF_THRESHOLD) {
                continue;
            }

            int classId = 0;
            float maxClassScore = detection[5];
            for (int c = 1; c < NUM_CLASSES; c++) {
                if (detection[5 + c] > maxClassScore) {
                    maxClassScore = detection[5 + c];
                    classId = c;
                }
            }

            float finalConfidence = confidence * maxClassScore;

            if (finalConfidence < CONF_THRESHOLD) {
                continue;
            }

            classHits[classId]++;

            // Координаты возвращаются в нормированных [0,1] относительно INPUT_SIZE
            // (т.е. относительно letterbox-картинки). Переводим в пиксели letterbox-картинки.
            float lbCenterX = detection[0] * INPUT_SIZE;
            float lbCenterY = detection[1] * INPUT_SIZE;
            float lbWidth = detection[2] * INPUT_SIZE;
            float lbHeight = detection[3] * INPUT_SIZE;

            // Снимаем letterbox padding и масштаб, чтобы получить координаты
            // в оригинальной картинке (исходный bitmap, переданный в isRedDigit).
            float centerX = (lbCenterX - lastLetterboxPadX) / lastLetterboxScale;
            float centerY = (lbCenterY - lastLetterboxPadY) / lastLetterboxScale;
            float width = lbWidth / lastLetterboxScale;
            float height = lbHeight / lastLetterboxScale;

            RectF box = new RectF(
                    centerX - width / 2,
                    centerY - height / 2,
                    centerX + width / 2,
                    centerY + height / 2
            );

            detections.add(new Detection(box, classId, finalConfidence));
        }

        Log.d(TAG, "Class distribution:");
        String[] classNames = {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "liters", "."};
        for (int i = 0; i < NUM_CLASSES; i++) {
            if (classHits[i] > 0) {
                Log.d(TAG, "  Class " + i + " (" + classNames[i] + "): " + classHits[i]);
            }
        }

        return detections;
    }

    /**
     * Определяет, окрашена ли цифра в красный цвет.
     * Используется для отсечения красных хвостовых цифр на аналоговых счётчиках.
     */
    private boolean isRedDigit(Bitmap bitmap, RectF box) {
        try {
            // expandRatio = 0 - проверяем строго внутри bbox цифры, без захвата фона.
            int x = Math.max(0, (int) box.left);
            int y = Math.max(0, (int) box.top);
            int x2 = Math.min(bitmap.getWidth(), (int) box.right);
            int y2 = Math.min(bitmap.getHeight(), (int) box.bottom);

            int width = x2 - x;
            int height = y2 - y;

            if (width <= 0 || height <= 0) {
                return false;
            }

            int redPixels = 0;
            int totalPixels = 0;
            float[] hsv = new float[3];

            for (int py = y; py < y2; py += 2) {
                for (int px = x; px < x2; px += 2) {
                    if (px >= bitmap.getWidth() || py >= bitmap.getHeight()) continue;

                    int pixel = bitmap.getPixel(px, py);
                    android.graphics.Color.colorToHSV(pixel, hsv);

                    float hue = hsv[0];        // 0-360
                    float saturation = hsv[1]; // 0-1
                    float value = hsv[2];      // 0-1

                    boolean isRed = ((hue >= 0 && hue <= 40) || (hue >= 320 && hue <= 360))
                            && saturation > 0.2 && value > 0.15;

                    if (isRed) {
                        redPixels++;
                    }
                    totalPixels++;
                }
            }

            float redRatio = totalPixels > 0 ? (float) redPixels / totalPixels : 0;

            Log.d(TAG, "      Red (HSV): " + redPixels + "/" + totalPixels +
                    " = " + String.format("%.1f%%", redRatio * 100) + " (threshold: 5%)");

            return redRatio > 0.05;

        } catch (Exception e) {
            Log.e(TAG, "Error checking red", e);
            return false;
        }
    }

    private List<Detection> nms(List<Detection> detections) {
        Collections.sort(detections, (a, b) -> Float.compare(b.confidence, a.confidence));
        List<Detection> result = new ArrayList<>();

        while (!detections.isEmpty()) {
            Detection best = detections.remove(0);
            result.add(best);
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.N) {
                detections.removeIf(det -> iou(best.box, det.box) > IOU_THRESHOLD);
            }
        }

        return result;
    }

    private float iou(RectF a, RectF b) {
        float intersectLeft = Math.max(a.left, b.left);
        float intersectTop = Math.max(a.top, b.top);
        float intersectRight = Math.min(a.right, b.right);
        float intersectBottom = Math.min(a.bottom, b.bottom);

        if (intersectRight < intersectLeft || intersectBottom < intersectTop) {
            return 0;
        }

        float intersectArea = (intersectRight - intersectLeft) * (intersectBottom - intersectTop);
        float aArea = a.width() * a.height();
        float bArea = b.width() * b.height();
        float unionArea = aArea + bArea - intersectArea;

        return intersectArea / unionArea;
    }

    private MappedByteBuffer loadModelFile() throws IOException {
        AssetFileDescriptor fileDescriptor = cordova.getActivity().getAssets().openFd("www/yolov5_meter.tflite");
        FileInputStream inputStream = new FileInputStream(fileDescriptor.getFileDescriptor());
        FileChannel fileChannel = inputStream.getChannel();
        long startOffset = fileDescriptor.getStartOffset();
        long declaredLength = fileDescriptor.getDeclaredLength();
        return fileChannel.map(FileChannel.MapMode.READ_ONLY, startOffset, declaredLength);
    }

    private static class Detection {
        RectF box;
        int classId;
        float confidence;

        Detection(RectF box, int classId, float confidence) {
            this.box = box;
            this.classId = classId;
            this.confidence = confidence;
        }
    }

    @Override
    public void onDestroy() {
        if (tflite != null) {
            tflite.close();
        }
        super.onDestroy();
    }
}