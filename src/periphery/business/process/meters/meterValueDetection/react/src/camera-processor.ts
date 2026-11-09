import type { CameraView } from "expo-camera";

export type CaptureFrameOptions = {
  quality?: number;
  skipProcessing?: boolean;
};

/**
 * Получение JPEG-кадра камеры без data URL-префикса.
 */
export async function captureCameraFrame(
  camera: CameraView,
  options: CaptureFrameOptions = {},
): Promise<string> {
  const picture = await camera.takePictureAsync({
    base64: true,
    quality: options.quality ?? 0.85,
    skipProcessing: options.skipProcessing ?? false,
  });

  if (!picture?.base64) {
    throw new Error("Камера не вернула изображение");
  }

  return picture.base64;
}
