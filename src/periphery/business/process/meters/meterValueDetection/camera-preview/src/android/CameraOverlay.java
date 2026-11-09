package com.cordovaplugincamerapreview;

import android.content.Context;
import android.graphics.Canvas;
import android.graphics.Color;
import android.graphics.Paint;
import android.graphics.Path;
import android.graphics.RectF;
import android.util.AttributeSet;
import android.util.Log;
import android.view.MotionEvent;
import android.view.View;

public class CameraOverlay extends View {
    private static final String TAG = "CameraOverlay";

    private Paint framePaint;
    private Paint shadowPaint;
    private Paint handlePaint;
    private Paint textPaint;
    private Paint textBackgroundPaint;
    private RectF frameRect;
    private Path shadowPath;

    private String overlayText = "Поместите показания счетчика внутрь рамки";

    // Режим взаимодействия
    private static final int MODE_NONE = 0;
    private static final int MODE_DRAG = 1;
    private static final int MODE_RESIZE_TL = 2; // top-left
    private static final int MODE_RESIZE_TR = 3; // top-right
    private static final int MODE_RESIZE_BL = 4; // bottom-left
    private static final int MODE_RESIZE_BR = 5; // bottom-right
    private static final int MODE_RESIZE_T = 6; // top edge
    private static final int MODE_RESIZE_B = 7; // bottom edge
    private static final int MODE_RESIZE_L = 8; // left edge
    private static final int MODE_RESIZE_R = 9; // right edge

    private int currentMode = MODE_NONE;
    private float touchStartX, touchStartY;
    private RectF touchStartRect = new RectF();

    // Зона касания для краёв/углов (в пикселях)
    private static final float HANDLE_TOUCH_RADIUS = 60f;
    // Размер визуальных маркеров на углах
    private static final float HANDLE_DRAW_SIZE = 20f;
    // Минимальный размер рамки (в пикселях)
    private static final float MIN_FRAME_WIDTH = 200f;
    private static final float MIN_FRAME_HEIGHT = 100f;

    private boolean frameInitialized = false;

    // Относительные координаты рамки (0.0 - 1.0) для передачи в JS
    private float frameRelLeft = 0.1f;
    private float frameRelTop = 0.35f;
    private float frameRelWidth = 0.8f;
    private float frameRelHeight = 0.3f;

    public CameraOverlay(Context context) {
        super(context);
        init();
    }

    public CameraOverlay(Context context, AttributeSet attrs) {
        super(context, attrs);
        init();
    }

    private void init() {
        framePaint = new Paint();
        framePaint.setColor(Color.parseColor("#f2711c"));
        framePaint.setStyle(Paint.Style.STROKE);
        framePaint.setStrokeWidth(6f);
        framePaint.setAntiAlias(true);

        shadowPaint = new Paint();
        shadowPaint.setColor(Color.parseColor("#B3000000"));
        shadowPaint.setStyle(Paint.Style.FILL);
        shadowPaint.setAntiAlias(true);

        handlePaint = new Paint();
        handlePaint.setColor(Color.parseColor("#f2711c"));
        handlePaint.setStyle(Paint.Style.FILL);
        handlePaint.setAntiAlias(true);

        textPaint = new Paint();
        textPaint.setColor(Color.WHITE);
        textPaint.setTextSize(40f);
        textPaint.setAntiAlias(true);
        textPaint.setTextAlign(Paint.Align.CENTER);

        textBackgroundPaint = new Paint();
        textBackgroundPaint.setColor(Color.parseColor("#E6f2711c"));
        textBackgroundPaint.setStyle(Paint.Style.FILL);
        textBackgroundPaint.setAntiAlias(true);

        frameRect = new RectF();
        shadowPath = new Path();

        setBackgroundColor(Color.TRANSPARENT);
    }

    public void setOverlayText(String text) {
        this.overlayText = text;
        invalidate();
    }

    /**
     * Возвращает относительные координаты рамки (0.0 - 1.0) для использования в JS.
     * Формат: [left, top, width, height] относительно размера view.
     */
    public float[] getFrameRelativeCoords() {
        return new float[] { frameRelLeft, frameRelTop, frameRelWidth, frameRelHeight };
    }

    @Override
    protected void onDraw(Canvas canvas) {
        super.onDraw(canvas);

        int width = getWidth();
        int height = getHeight();

        if (width == 0 || height == 0) {
            return;
        }

        if (!frameInitialized) {
            frameRect.set(
                    width * frameRelLeft,
                    height * frameRelTop,
                    width * (frameRelLeft + frameRelWidth),
                    height * (frameRelTop + frameRelHeight)
            );
            frameInitialized = true;
        }

        // Тень вокруг рамки
        shadowPath.reset();
        shadowPath.addRect(0, 0, width, height, Path.Direction.CW);
        shadowPath.addRoundRect(frameRect, 24f, 24f, Path.Direction.CCW);
        canvas.drawPath(shadowPath, shadowPaint);

        // Рамка
        canvas.drawRoundRect(frameRect, 24f, 24f, framePaint);

        // Маркеры на углах
        drawHandle(canvas, frameRect.left, frameRect.top);
        drawHandle(canvas, frameRect.right, frameRect.top);
        drawHandle(canvas, frameRect.left, frameRect.bottom);
        drawHandle(canvas, frameRect.right, frameRect.bottom);

        // Маркеры на серединах краёв
        drawHandle(canvas, frameRect.centerX(), frameRect.top);
        drawHandle(canvas, frameRect.centerX(), frameRect.bottom);
        drawHandle(canvas, frameRect.left, frameRect.centerY());
        drawHandle(canvas, frameRect.right, frameRect.centerY());

        // Текст над рамкой
        float centerX = width / 2f;
        if (overlayText != null && !overlayText.isEmpty()) {
            float textY = frameRect.top - 80f;
            if (textY < 60f) {
                textY = frameRect.bottom + 80f;
            }

            float textWidth = textPaint.measureText(overlayText);
            float padding = 40f;

            RectF textBackground = new RectF(
                    centerX - textWidth / 2f - padding,
                    textY - 60f,
                    centerX + textWidth / 2f + padding,
                    textY + 20f
            );
            canvas.drawRoundRect(textBackground, 50f, 50f, textBackgroundPaint);
            canvas.drawText(overlayText, centerX, textY, textPaint);
        }
    }

    private void drawHandle(Canvas canvas, float cx, float cy) {
        canvas.drawRoundRect(
                cx - HANDLE_DRAW_SIZE, cy - HANDLE_DRAW_SIZE,
                cx + HANDLE_DRAW_SIZE, cy + HANDLE_DRAW_SIZE,
                6f, 6f, handlePaint
        );
    }

    @Override
    public boolean onTouchEvent(MotionEvent event) {
        float x = event.getX();
        float y = event.getY();

        switch (event.getAction()) {
            case MotionEvent.ACTION_DOWN:
                currentMode = detectMode(x, y);
                if (currentMode != MODE_NONE) {
                    touchStartX = x;
                    touchStartY = y;
                    touchStartRect.set(frameRect);
                    return true;
                }
                return false;

            case MotionEvent.ACTION_MOVE:
                if (currentMode == MODE_NONE) return false;
                handleMove(x, y);
                invalidate();
                return true;

            case MotionEvent.ACTION_UP:
            case MotionEvent.ACTION_CANCEL:
                if (currentMode != MODE_NONE) {
                    currentMode = MODE_NONE;
                    updateRelativeCoords();
                    Log.d(TAG, "Frame updated: " +
                            String.format("%.1f%% x %.1f%% at (%.1f%%, %.1f%%)",
                                    frameRelWidth * 100, frameRelHeight * 100,
                                    frameRelLeft * 100, frameRelTop * 100));
                    return true;
                }
                return false;
        }

        return super.onTouchEvent(event);
    }

    /**
     * Определяет режим взаимодействия по координатам нажатия.
     * Приоритет: углы > края > внутри рамки (drag) > снаружи (ничего).
     */
    private int detectMode(float x, float y) {
        float r = HANDLE_TOUCH_RADIUS;

        // Углы
        if (near(x, y, frameRect.left, frameRect.top, r)) return MODE_RESIZE_TL;
        if (near(x, y, frameRect.right, frameRect.top, r)) return MODE_RESIZE_TR;
        if (near(x, y, frameRect.left, frameRect.bottom, r)) return MODE_RESIZE_BL;
        if (near(x, y, frameRect.right, frameRect.bottom, r)) return MODE_RESIZE_BR;

        // Края (середины)
        if (near(x, y, frameRect.centerX(), frameRect.top, r)) return MODE_RESIZE_T;
        if (near(x, y, frameRect.centerX(), frameRect.bottom, r)) return MODE_RESIZE_B;
        if (near(x, y, frameRect.left, frameRect.centerY(), r)) return MODE_RESIZE_L;
        if (near(x, y, frameRect.right, frameRect.centerY(), r)) return MODE_RESIZE_R;

        // Любое место на краю рамки (расширенная зона)
        if (nearEdge(x, y, frameRect, r)) {
            // Определяем к какому краю ближе
            float dLeft = Math.abs(x - frameRect.left);
            float dRight = Math.abs(x - frameRect.right);
            float dTop = Math.abs(y - frameRect.top);
            float dBottom = Math.abs(y - frameRect.bottom);
            float min = Math.min(Math.min(dLeft, dRight), Math.min(dTop, dBottom));

            if (min == dLeft) return MODE_RESIZE_L;
            if (min == dRight) return MODE_RESIZE_R;
            if (min == dTop) return MODE_RESIZE_T;
            return MODE_RESIZE_B;
        }

        // Внутри рамки - drag
        if (frameRect.contains(x, y)) return MODE_DRAG;

        return MODE_NONE;
    }

    private boolean near(float x, float y, float cx, float cy, float radius) {
        return Math.abs(x - cx) < radius && Math.abs(y - cy) < radius;
    }

    private boolean nearEdge(float x, float y, RectF rect, float radius) {
        boolean nearHorizontalEdge = (Math.abs(y - rect.top) < radius || Math.abs(y - rect.bottom) < radius)
                && x >= rect.left - radius && x <= rect.right + radius;
        boolean nearVerticalEdge = (Math.abs(x - rect.left) < radius || Math.abs(x - rect.right) < radius)
                && y >= rect.top - radius && y <= rect.bottom + radius;
        return nearHorizontalEdge || nearVerticalEdge;
    }

    private void handleMove(float x, float y) {
        float dx = x - touchStartX;
        float dy = y - touchStartY;
        int w = getWidth();
        int h = getHeight();

        switch (currentMode) {
            case MODE_DRAG: {
                float newLeft = touchStartRect.left + dx;
                float newTop = touchStartRect.top + dy;
                float fw = touchStartRect.width();
                float fh = touchStartRect.height();

                // Ограничиваем рамкой view
                newLeft = Math.max(0, Math.min(newLeft, w - fw));
                newTop = Math.max(0, Math.min(newTop, h - fh));

                frameRect.set(newLeft, newTop, newLeft + fw, newTop + fh);
                break;
            }

            case MODE_RESIZE_TL:
                resizeFrame(touchStartRect.left + dx, touchStartRect.top + dy,
                        touchStartRect.right, touchStartRect.bottom, w, h);
                break;
            case MODE_RESIZE_TR:
                resizeFrame(touchStartRect.left, touchStartRect.top + dy,
                        touchStartRect.right + dx, touchStartRect.bottom, w, h);
                break;
            case MODE_RESIZE_BL:
                resizeFrame(touchStartRect.left + dx, touchStartRect.top,
                        touchStartRect.right, touchStartRect.bottom + dy, w, h);
                break;
            case MODE_RESIZE_BR:
                resizeFrame(touchStartRect.left, touchStartRect.top,
                        touchStartRect.right + dx, touchStartRect.bottom + dy, w, h);
                break;

            case MODE_RESIZE_T:
                resizeFrame(touchStartRect.left, touchStartRect.top + dy,
                        touchStartRect.right, touchStartRect.bottom, w, h);
                break;
            case MODE_RESIZE_B:
                resizeFrame(touchStartRect.left, touchStartRect.top,
                        touchStartRect.right, touchStartRect.bottom + dy, w, h);
                break;
            case MODE_RESIZE_L:
                resizeFrame(touchStartRect.left + dx, touchStartRect.top,
                        touchStartRect.right, touchStartRect.bottom, w, h);
                break;
            case MODE_RESIZE_R:
                resizeFrame(touchStartRect.left, touchStartRect.top,
                        touchStartRect.right + dx, touchStartRect.bottom, w, h);
                break;
        }
    }

    private void resizeFrame(float left, float top, float right, float bottom, int viewW, int viewH) {
        // Ограничиваем границами view
        left = Math.max(0, left);
        top = Math.max(0, top);
        right = Math.min(viewW, right);
        bottom = Math.min(viewH, bottom);

        // Минимальный размер
        if (right - left < MIN_FRAME_WIDTH) {
            if (left != touchStartRect.left) {
                left = right - MIN_FRAME_WIDTH;
            } else {
                right = left + MIN_FRAME_WIDTH;
            }
        }
        if (bottom - top < MIN_FRAME_HEIGHT) {
            if (top != touchStartRect.top) {
                top = bottom - MIN_FRAME_HEIGHT;
            } else {
                bottom = top + MIN_FRAME_HEIGHT;
            }
        }

        // Ещё раз проверяем границы после корректировки минимума
        left = Math.max(0, left);
        top = Math.max(0, top);
        right = Math.min(viewW, right);
        bottom = Math.min(viewH, bottom);

        frameRect.set(left, top, right, bottom);
    }

    /**
     * Обновляет относительные координаты после изменения рамки.
     */
    private void updateRelativeCoords() {
        int w = getWidth();
        int h = getHeight();
        if (w > 0 && h > 0) {
            frameRelLeft = frameRect.left / w;
            frameRelTop = frameRect.top / h;
            frameRelWidth = frameRect.width() / w;
            frameRelHeight = frameRect.height() / h;
        }
    }
}