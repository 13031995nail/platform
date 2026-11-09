import type {
  MeterRecognitionCropRegion,
  MeterRecognitionResult,
} from "./MeterRecognition.types";

export function isMeterRecognitionAvailable(): boolean {
  return false;
}

export async function detectDigits(
  _base64Image: string,
  _cropRegion?: MeterRecognitionCropRegion,
): Promise<MeterRecognitionResult> {
  throw new Error(
    "Распознавание показаний поддерживается только в Android-приложении.",
  );
}

export type {
  MeterRecognitionCropRegion,
  MeterRecognitionResult,
} from "./MeterRecognition.types";
