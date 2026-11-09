import { Ionicons } from "@expo/vector-icons";
import { CameraView, useCameraPermissions } from "expo-camera";
import { useCallback, useEffect, useRef, useState } from "react";
import {
  ActivityIndicator,
  Image,
  Modal,
  Platform,
  Pressable,
  StyleSheet,
  Text,
  View,
} from "react-native";

import { isMeterRecognitionAvailable } from "./MeterRecognitionModule";
import type { MeterRecognitionCropRegion } from "./MeterRecognition.types";
import { captureCameraFrame } from "./camera-processor";
import {
  detectMeterValue,
  MeterDetectionSession,
  type StableMeterRecognitionResult,
} from "./meter-detection";

const METER_GUIDE_REGION: MeterRecognitionCropRegion = {
  left: 0.08,
  top: 0.34,
  width: 0.84,
  height: 0.25,
};

export type MeterValueDetectionCameraProps = {
  buttonCaption?: string;
  readonly?: boolean;
  hidePreview?: boolean;
  intervalMs?: number;
  maxResults?: number;
  minLength?: number;
  onResult?: (
    value: string | null,
    confidence: number | null,
    imageDataUrl: string,
  ) => void;
};

/**
 * Открытие камеры, live-распознавание и возврат финального снимка со значением.
 */
export function MeterValueDetectionCamera({
  buttonCaption = "Распознать показания",
  readonly = false,
  hidePreview = false,
  intervalMs = 1500,
  maxResults = 5,
  minLength = 0,
  onResult,
}: MeterValueDetectionCameraProps) {
  const cameraRef = useRef<CameraView | null>(null);
  const detectionSession = useRef(new MeterDetectionSession());
  const liveResultRef = useRef<StableMeterRecognitionResult | null>(null);
  const [permission, requestPermission] = useCameraPermissions();
  const [isOpen, setIsOpen] = useState(false);
  const [isCameraReady, setIsCameraReady] = useState(false);
  const [isBusy, setIsBusy] = useState(false);
  const [isTorchEnabled, setIsTorchEnabled] = useState(false);
  const [liveResult, setLiveResult] =
    useState<StableMeterRecognitionResult | null>(null);
  const [capturedImage, setCapturedImage] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  const stopDetection = useCallback(() => {
    detectionSession.current.stop();
  }, []);

  const closeCamera = useCallback(() => {
    stopDetection();
    setIsOpen(false);
    setIsCameraReady(false);
    setIsTorchEnabled(false);
    setIsBusy(false);
  }, [stopDetection]);

  useEffect(() => {
    if (!isOpen || !isCameraReady || !permission?.granted) {
      return;
    }

    detectionSession.current.start(
      async () => {
        const camera = cameraRef.current;
        if (!camera) {
          return null;
        }
        return captureCameraFrame(camera, {
          quality: 0.7,
        });
      },
      (result) => {
        liveResultRef.current = result;
        setLiveResult(result);
      },
      {
        intervalMs,
        maxResults,
        minLength,
        cropRegion: METER_GUIDE_REGION,
      },
    );

    return stopDetection;
  }, [
    intervalMs,
    isCameraReady,
    isOpen,
    maxResults,
    minLength,
    permission?.granted,
    stopDetection,
  ]);

  useEffect(() => stopDetection, [stopDetection]);

  const openCamera = async () => {
    setError(null);
    setCapturedImage(null);
    setLiveResult(null);
    liveResultRef.current = null;

    if (Platform.OS !== "android" || !isMeterRecognitionAvailable()) {
      setError(
        "Распознавание доступно в Android development build. Expo Go и web не содержат нативную модель.",
      );
      return;
    }

    const currentPermission = permission?.granted
      ? permission
      : await requestPermission();
    if (!currentPermission.granted) {
      setError("Для распознавания необходимо разрешить доступ к камере.");
      return;
    }

    setIsOpen(true);
  };

  const takePicture = async () => {
    const camera = cameraRef.current;
    if (!camera || isBusy) {
      return;
    }

    setIsBusy(true);

    try {
      await detectionSession.current.stopAndWait(false);
      const base64Image = await captureCameraFrame(camera, { quality: 0.9 });
      const imageDataUrl = `data:image/jpeg;base64,${base64Image}`;
      let result = liveResultRef.current;

      try {
        const finalResult = await detectMeterValue(
          base64Image,
          METER_GUIDE_REGION,
        );
        if (finalResult.text || finalResult.wholePart) {
          result = detectionSession.current.addResult(finalResult);
        }
      } catch (recognitionError) {
        console.error(
          "[MeterValueDetectionCamera] Ошибка финального распознавания:",
          recognitionError,
        );
      }

      if (!hidePreview) {
        setCapturedImage(imageDataUrl);
      }
      onResult?.(
        result?.text || null,
        result?.confidence ?? null,
        imageDataUrl,
      );
      closeCamera();
    } catch (captureError) {
      console.error("[MeterValueDetectionCamera] Ошибка съёмки:", captureError);
      setError("Не удалось получить снимок с камеры.");
      closeCamera();
    }
  };

  if (readonly) {
    return capturedImage && !hidePreview ? (
      <Image source={{ uri: capturedImage }} style={styles.preview} />
    ) : null;
  }

  return (
    <View style={styles.container}>
      <Pressable style={styles.openButton} onPress={() => void openCamera()}>
        <Ionicons name="camera-outline" size={22} color="#fff" />
        <Text style={styles.openButtonText}>{buttonCaption}</Text>
      </Pressable>

      {error ? <Text style={styles.errorText}>{error}</Text> : null}
      {capturedImage && !hidePreview ? (
        <Image source={{ uri: capturedImage }} style={styles.preview} />
      ) : null}

      <Modal
        visible={isOpen}
        animationType="slide"
        onRequestClose={closeCamera}
      >
        <View style={styles.cameraContainer}>
          <CameraView
            ref={cameraRef}
            style={StyleSheet.absoluteFill}
            facing="back"
            enableTorch={isTorchEnabled}
            animateShutter={false}
            onCameraReady={() => setIsCameraReady(true)}
            onMountError={({ message }) => setError(message)}
          />

          <View pointerEvents="none" style={styles.guide} />

          <View style={styles.liveResult}>
            <Text style={styles.liveValue}>
              {liveResult?.text || "Наведите камеру на показания"}
            </Text>
            {liveResult ? (
              <Text style={styles.liveConfidence}>
                Уверенность: {liveResult.confidence}%
                {liveResult.isReliable ? "" : " · проверка"}
              </Text>
            ) : null}
          </View>

          <View style={styles.controls}>
            <Pressable style={styles.controlButton} onPress={closeCamera}>
              <Ionicons name="close" size={30} color="#fff" />
            </Pressable>
            <Pressable
              style={[styles.captureButton, isBusy && styles.disabledButton]}
              disabled={!isCameraReady || isBusy}
              onPress={() => void takePicture()}
            >
              {isBusy ? <ActivityIndicator color="#208aef" /> : null}
            </Pressable>
            <Pressable
              style={styles.controlButton}
              onPress={() => setIsTorchEnabled((value) => !value)}
            >
              <Ionicons
                name={isTorchEnabled ? "flash" : "flash-off"}
                size={27}
                color="#fff"
              />
            </Pressable>
          </View>
        </View>
      </Modal>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    gap: 12,
    width: "100%",
  },
  openButton: {
    alignItems: "center",
    alignSelf: "center",
    backgroundColor: "#208aef",
    borderRadius: 12,
    flexDirection: "row",
    gap: 8,
    paddingHorizontal: 18,
    paddingVertical: 12,
  },
  openButtonText: {
    color: "#fff",
    fontSize: 16,
    fontWeight: "600",
  },
  errorText: {
    color: "#c62828",
    textAlign: "center",
  },
  preview: {
    aspectRatio: 4 / 3,
    borderRadius: 12,
    width: "100%",
  },
  cameraContainer: {
    backgroundColor: "#000",
    flex: 1,
  },
  guide: {
    borderColor: "#fff",
    borderRadius: 12,
    borderWidth: 2,
    height: toPercent(METER_GUIDE_REGION.height),
    left: toPercent(METER_GUIDE_REGION.left),
    position: "absolute",
    top: toPercent(METER_GUIDE_REGION.top),
    width: toPercent(METER_GUIDE_REGION.width),
  },
  liveResult: {
    alignItems: "center",
    backgroundColor: "rgba(0, 0, 0, 0.62)",
    borderRadius: 12,
    left: 20,
    padding: 12,
    position: "absolute",
    right: 20,
    top: 48,
  },
  liveValue: {
    color: "#fff",
    fontSize: 24,
    fontWeight: "700",
  },
  liveConfidence: {
    color: "#fff",
    marginTop: 4,
  },
  controls: {
    alignItems: "center",
    bottom: 40,
    flexDirection: "row",
    justifyContent: "space-around",
    left: 20,
    position: "absolute",
    right: 20,
  },
  controlButton: {
    alignItems: "center",
    backgroundColor: "rgba(0, 0, 0, 0.55)",
    borderRadius: 28,
    height: 56,
    justifyContent: "center",
    width: 56,
  },
  captureButton: {
    alignItems: "center",
    backgroundColor: "#fff",
    borderColor: "rgba(255, 255, 255, 0.55)",
    borderRadius: 38,
    borderWidth: 6,
    height: 76,
    justifyContent: "center",
    width: 76,
  },
  disabledButton: {
    opacity: 0.65,
  },
});

function toPercent(value: number): `${number}%` {
  return `${value * 100}%`;
}
