package expo.modules.meterrecognition;

import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Canvas;
import android.graphics.Color;
import android.graphics.RectF;
import android.util.Base64;
import android.util.Log;

import org.tensorflow.lite.Interpreter;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.util.ArrayList;
import java.util.Comparator;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;

final class MeterRecognizer implements AutoCloseable {
    private static final String TAG = "MeterRecognition";
    private static final String MODEL_ASSET = "yolov5_meter.tflite";
    private static final int INPUT_SIZE = 416;
    private static final int NUM_CLASSES = 12;
    private static final int OUTPUT_ROWS = 10647;
    private static final int OUTPUT_COLUMNS = 17;
    private static final int MAX_BASE64_LENGTH = 32 * 1024 * 1024;
    private static final float CONF_THRESHOLD = 0.25f;
    private static final float IOU_THRESHOLD = 0.45f;

    private final ByteBuffer modelBuffer;
    private final Interpreter interpreter;
    private float lastLetterboxScale = 1.0f;
    private float lastLetterboxPadX;
    private float lastLetterboxPadY;

    MeterRecognizer(Context context) throws IOException {
        Interpreter.Options options = new Interpreter.Options();
        options.setNumThreads(4);
        modelBuffer = loadModel(context);
        interpreter = new Interpreter(modelBuffer, options);
        Log.d(TAG, "YOLOv5 model loaded successfully");
    }

    Map<String, Object> detectDigits(
            String base64Image,
            Float cropLeft,
            Float cropTop,
            Float cropWidth,
            Float cropHeight
    ) {
        if (base64Image == null || base64Image.trim().isEmpty()) {
            throw new IllegalArgumentException("Image is empty");
        }

        String normalizedImage = stripDataUrlPrefix(base64Image);
        if (normalizedImage.length() > MAX_BASE64_LENGTH) {
            throw new IllegalArgumentException("Image is too large");
        }

        byte[] decodedImage;
        try {
            decodedImage = Base64.decode(normalizedImage, Base64.DEFAULT);
        } catch (IllegalArgumentException error) {
            throw new IllegalArgumentException("Image is not valid base64", error);
        }

        Bitmap decodedBitmap = BitmapFactory.decodeByteArray(decodedImage, 0, decodedImage.length);
        if (decodedBitmap == null) {
            throw new IllegalArgumentException("Failed to decode image");
        }

        Bitmap recognitionBitmap = null;
        try {
            recognitionBitmap = cropBitmap(
                    decodedBitmap,
                    cropLeft,
                    cropTop,
                    cropWidth,
                    cropHeight
            );
            ByteBuffer inputBuffer = preprocessImage(recognitionBitmap);
            float[][][] output = new float[1][OUTPUT_ROWS][OUTPUT_COLUMNS];

            interpreter.run(inputBuffer, output);

            List<Detection> detections = parseOutput(output[0]);
            List<Detection> filtered = nms(detections);
            filtered.sort(Comparator.comparingDouble(detection -> detection.box.left));

            return buildResult(recognitionBitmap, filtered);
        } finally {
            if (recognitionBitmap != null && recognitionBitmap != decodedBitmap) {
                recognitionBitmap.recycle();
            }
            decodedBitmap.recycle();
        }
    }

    private Bitmap cropBitmap(
            Bitmap source,
            Float left,
            Float top,
            Float width,
            Float height
    ) {
        if (left == null && top == null && width == null && height == null) {
            return source;
        }
        if (!isValidCropValue(left)
                || !isValidCropValue(top)
                || !isValidCropValue(width)
                || !isValidCropValue(height)
                || width <= 0
                || height <= 0
                || left + width > 1
                || top + height > 1) {
            throw new IllegalArgumentException("Crop region must be within the image bounds");
        }

        int sourceWidth = source.getWidth();
        int sourceHeight = source.getHeight();
        int cropLeft = Math.min(sourceWidth - 1, Math.round(left * sourceWidth));
        int cropTop = Math.min(sourceHeight - 1, Math.round(top * sourceHeight));
        int cropWidth = Math.min(
                sourceWidth - cropLeft,
                Math.max(1, Math.round(width * sourceWidth))
        );
        int cropHeight = Math.min(
                sourceHeight - cropTop,
                Math.max(1, Math.round(height * sourceHeight))
        );

        if (cropLeft == 0
                && cropTop == 0
                && cropWidth == sourceWidth
                && cropHeight == sourceHeight) {
            return source;
        }
        return Bitmap.createBitmap(source, cropLeft, cropTop, cropWidth, cropHeight);
    }

    private boolean isValidCropValue(Float value) {
        return value != null
                && !Float.isNaN(value)
                && !Float.isInfinite(value)
                && value >= 0
                && value <= 1;
    }

    private Map<String, Object> buildResult(Bitmap bitmap, List<Detection> filtered) {
        Detection dotDetection = null;
        List<Detection> digitDetections = new ArrayList<>();

        for (Detection detection : filtered) {
            if (detection.classId == 11) {
                dotDetection = detection;
            } else if (detection.classId >= 0 && detection.classId <= 9) {
                digitDetections.add(detection);
            }
        }

        List<Detection> validDigits = removeRedTailDigits(bitmap, digitDetections);
        StringBuilder wholePart = new StringBuilder();
        StringBuilder fractionalPart = new StringBuilder();

        if (dotDetection != null) {
            float dotX = dotDetection.box.centerX();
            for (Detection detection : validDigits) {
                if (detection.box.centerX() < dotX) {
                    wholePart.append(detection.classId);
                } else {
                    fractionalPart.append(detection.classId);
                }
            }
        } else {
            for (Detection detection : validDigits) {
                wholePart.append(detection.classId);
            }
        }

        String wholePartValue = wholePart.toString();
        if (wholePartValue.length() > 1) {
            wholePartValue = wholePartValue.replaceFirst("^0+(?!$)", "");
            if (wholePartValue.isEmpty()) {
                wholePartValue = "0";
            }
        }

        String fractionalPartValue = fractionalPart.toString();
        String text;
        if (dotDetection != null && !fractionalPartValue.isEmpty()) {
            text = wholePartValue + "." + fractionalPartValue;
        } else if (dotDetection != null) {
            text = wholePartValue + ".";
        } else {
            text = wholePartValue;
        }

        int confidence = averageConfidence(validDigits);
        Map<String, Object> result = new HashMap<>();
        result.put("text", text);
        result.put("wholePart", wholePartValue);
        result.put("fractionalPart", fractionalPartValue);
        result.put("hasDot", dotDetection != null);
        result.put("confidence", confidence);
        result.put("digitCount", validDigits.size());

        Log.d(TAG, "Detected: '" + text + "' (" + validDigits.size() + " digits)");
        return result;
    }

    private List<Detection> removeRedTailDigits(Bitmap bitmap, List<Detection> digits) {
        int removeFromIndex = digits.size();
        for (int index = digits.size() - 1; index >= 0; index--) {
            if (isRedDigit(bitmap, digits.get(index).box)) {
                removeFromIndex = index;
            } else {
                break;
            }
        }

        return new ArrayList<>(digits.subList(0, removeFromIndex));
    }

    private int averageConfidence(List<Detection> detections) {
        if (detections.isEmpty()) {
            return 0;
        }

        double sum = 0;
        for (Detection detection : detections) {
            sum += detection.confidence;
        }
        return (int) ((sum / detections.size()) * 100);
    }

    private Bitmap letterbox(Bitmap source) {
        int sourceWidth = source.getWidth();
        int sourceHeight = source.getHeight();
        float scale = Math.min(
                (float) INPUT_SIZE / sourceWidth,
                (float) INPUT_SIZE / sourceHeight
        );
        int scaledWidth = Math.round(sourceWidth * scale);
        int scaledHeight = Math.round(sourceHeight * scale);
        int padX = (INPUT_SIZE - scaledWidth) / 2;
        int padY = (INPUT_SIZE - scaledHeight) / 2;

        lastLetterboxScale = scale;
        lastLetterboxPadX = padX;
        lastLetterboxPadY = padY;

        Bitmap result = Bitmap.createBitmap(INPUT_SIZE, INPUT_SIZE, Bitmap.Config.ARGB_8888);
        Canvas canvas = new Canvas(result);
        canvas.drawColor(Color.rgb(114, 114, 114));

        Bitmap scaled = Bitmap.createScaledBitmap(source, scaledWidth, scaledHeight, true);
        canvas.drawBitmap(scaled, padX, padY, null);
        if (scaled != source) {
            scaled.recycle();
        }

        return result;
    }

    private ByteBuffer preprocessImage(Bitmap bitmap) {
        Bitmap letterboxed = letterbox(bitmap);
        try {
            ByteBuffer buffer = ByteBuffer.allocateDirect(
                    Float.BYTES * INPUT_SIZE * INPUT_SIZE * 3
            );
            buffer.order(ByteOrder.nativeOrder());

            int[] pixels = new int[INPUT_SIZE * INPUT_SIZE];
            letterboxed.getPixels(pixels, 0, INPUT_SIZE, 0, 0, INPUT_SIZE, INPUT_SIZE);
            for (int pixel : pixels) {
                buffer.putFloat(((pixel >> 16) & 0xff) / 255.0f);
                buffer.putFloat(((pixel >> 8) & 0xff) / 255.0f);
                buffer.putFloat((pixel & 0xff) / 255.0f);
            }
            buffer.rewind();
            return buffer;
        } finally {
            letterboxed.recycle();
        }
    }

    private List<Detection> parseOutput(float[][] output) {
        List<Detection> detections = new ArrayList<>();
        for (float[] candidate : output) {
            float objectConfidence = candidate[4];
            if (objectConfidence < CONF_THRESHOLD) {
                continue;
            }

            int classId = 0;
            float maxClassScore = candidate[5];
            for (int classIndex = 1; classIndex < NUM_CLASSES; classIndex++) {
                if (candidate[5 + classIndex] > maxClassScore) {
                    maxClassScore = candidate[5 + classIndex];
                    classId = classIndex;
                }
            }

            float confidence = objectConfidence * maxClassScore;
            if (confidence < CONF_THRESHOLD) {
                continue;
            }

            float letterboxCenterX = candidate[0] * INPUT_SIZE;
            float letterboxCenterY = candidate[1] * INPUT_SIZE;
            float width = candidate[2] * INPUT_SIZE / lastLetterboxScale;
            float height = candidate[3] * INPUT_SIZE / lastLetterboxScale;
            float centerX = (letterboxCenterX - lastLetterboxPadX) / lastLetterboxScale;
            float centerY = (letterboxCenterY - lastLetterboxPadY) / lastLetterboxScale;
            RectF box = new RectF(
                    centerX - width / 2,
                    centerY - height / 2,
                    centerX + width / 2,
                    centerY + height / 2
            );

            detections.add(new Detection(box, classId, confidence));
        }
        return detections;
    }

    private boolean isRedDigit(Bitmap bitmap, RectF box) {
        int left = Math.max(0, (int) box.left);
        int top = Math.max(0, (int) box.top);
        int right = Math.min(bitmap.getWidth(), (int) box.right);
        int bottom = Math.min(bitmap.getHeight(), (int) box.bottom);
        if (right <= left || bottom <= top) {
            return false;
        }

        int redPixels = 0;
        int totalPixels = 0;
        float[] hsv = new float[3];
        for (int y = top; y < bottom; y += 2) {
            for (int x = left; x < right; x += 2) {
                Color.colorToHSV(bitmap.getPixel(x, y), hsv);
                float hue = hsv[0];
                boolean isRed = ((hue >= 0 && hue <= 40) || (hue >= 320 && hue <= 360))
                        && hsv[1] > 0.2f
                        && hsv[2] > 0.15f;
                if (isRed) {
                    redPixels++;
                }
                totalPixels++;
            }
        }

        return totalPixels > 0 && (float) redPixels / totalPixels > 0.05f;
    }

    private List<Detection> nms(List<Detection> detections) {
        detections.sort((first, second) -> Float.compare(
                second.confidence,
                first.confidence
        ));
        List<Detection> result = new ArrayList<>();

        while (!detections.isEmpty()) {
            Detection best = detections.remove(0);
            result.add(best);
            Iterator<Detection> iterator = detections.iterator();
            while (iterator.hasNext()) {
                if (iou(best.box, iterator.next().box) > IOU_THRESHOLD) {
                    iterator.remove();
                }
            }
        }

        return result;
    }

    private float iou(RectF first, RectF second) {
        float intersectLeft = Math.max(first.left, second.left);
        float intersectTop = Math.max(first.top, second.top);
        float intersectRight = Math.min(first.right, second.right);
        float intersectBottom = Math.min(first.bottom, second.bottom);
        if (intersectRight <= intersectLeft || intersectBottom <= intersectTop) {
            return 0;
        }

        float intersectArea = (intersectRight - intersectLeft)
                * (intersectBottom - intersectTop);
        float unionArea = first.width() * first.height()
                + second.width() * second.height()
                - intersectArea;
        return unionArea > 0 ? intersectArea / unionArea : 0;
    }

    private static String stripDataUrlPrefix(String value) {
        int separatorIndex = value.indexOf(',');
        if (value.startsWith("data:image/") && separatorIndex >= 0) {
            return value.substring(separatorIndex + 1);
        }
        return value;
    }

    private static ByteBuffer loadModel(Context context) throws IOException {
        try (
                InputStream input = context.getAssets().open(MODEL_ASSET);
                ByteArrayOutputStream output = new ByteArrayOutputStream()
        ) {
            byte[] chunk = new byte[16 * 1024];
            int length;
            while ((length = input.read(chunk)) != -1) {
                output.write(chunk, 0, length);
            }
            byte[] bytes = output.toByteArray();
            ByteBuffer buffer = ByteBuffer.allocateDirect(bytes.length);
            buffer.order(ByteOrder.nativeOrder());
            buffer.put(bytes);
            buffer.rewind();
            return buffer;
        }
    }

    @Override
    public void close() {
        interpreter.close();
    }

    private static final class Detection {
        private final RectF box;
        private final int classId;
        private final float confidence;

        private Detection(RectF box, int classId, float confidence) {
            this.box = box;
            this.classId = classId;
            this.confidence = confidence;
        }
    }
}
