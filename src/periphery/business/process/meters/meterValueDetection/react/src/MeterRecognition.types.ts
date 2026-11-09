export type MeterRecognitionResult = {
  text: string;
  wholePart: string;
  fractionalPart: string;
  hasDot: boolean;
  confidence: number;
  digitCount: number;
};

/**
 * Нормализованная область изображения для распознавания.
 */
export type MeterRecognitionCropRegion = {
  left: number;
  top: number;
  width: number;
  height: number;
};
