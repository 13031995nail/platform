import { requireOptionalNativeModule } from "expo-modules-core";

import type {
  MeterRecognitionCropRegion,
  MeterRecognitionResult,
} from "./MeterRecognition.types";

type NativeMeterRecognitionModule = {
  detectDigits(
    base64Image: string,
    cropLeft: number | null,
    cropTop: number | null,
    cropWidth: number | null,
    cropHeight: number | null,
  ): Promise<MeterRecognitionResult>;
};

const nativeModule =
  requireOptionalNativeModule<NativeMeterRecognitionModule>("MeterRecognition");

export function isMeterRecognitionAvailable(): boolean {
  return nativeModule !== null;
}

export async function detectDigits(
  base64Image: string,
  cropRegion?: MeterRecognitionCropRegion,
): Promise<MeterRecognitionResult> {
  if (!nativeModule) {
    throw new Error(
      "MeterRecognition недоступен. Используйте Android development build, а не Expo Go.",
    );
  }

  return nativeModule.detectDigits(
    base64Image,
    cropRegion?.left ?? null,
    cropRegion?.top ?? null,
    cropRegion?.width ?? null,
    cropRegion?.height ?? null,
  );
}

export type {
  MeterRecognitionCropRegion,
  MeterRecognitionResult,
} from "./MeterRecognition.types";
