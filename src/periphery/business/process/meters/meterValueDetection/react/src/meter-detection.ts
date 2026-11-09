import {
  detectDigits,
  type MeterRecognitionCropRegion,
  type MeterRecognitionResult,
} from "./MeterRecognitionModule";

export type MeterDetectionSample = MeterRecognitionResult & {
  modelConfidence: number;
  timestamp: number;
};

export type StableMeterRecognitionResult = MeterRecognitionResult & {
  allResults: MeterDetectionSample[];
  isReliable: boolean;
};

export type ContinuousDetectionOptions = {
  intervalMs?: number;
  maxResults?: number;
  minLength?: number;
  cropRegion?: MeterRecognitionCropRegion;
};

const DEFAULT_INTERVAL_MS = 1500;
const DEFAULT_MAX_RESULTS = 5;

/**
 * Распознавание одного JPEG-кадра через Android Expo Module.
 */
export function detectMeterValue(
  image: string,
  cropRegion?: MeterRecognitionCropRegion,
): Promise<MeterRecognitionResult> {
  return detectDigits(stripDataUrlPrefix(image), cropRegion);
}

/**
 * Управление последовательным live-распознаванием с оценкой стабильности.
 */
export class MeterDetectionSession {
  private runId = 0;
  private timer: ReturnType<typeof setTimeout> | null = null;
  private results: MeterDetectionSample[] = [];
  private maxResults = DEFAULT_MAX_RESULTS;
  private inFlight: Promise<void> | null = null;

  start(
    getFrame: () => Promise<string | null>,
    onResult: (result: StableMeterRecognitionResult) => void,
    options: ContinuousDetectionOptions = {},
  ): void {
    this.stop();

    const intervalMs = Math.max(250, options.intervalMs ?? DEFAULT_INTERVAL_MS);
    this.maxResults = Math.max(1, options.maxResults ?? DEFAULT_MAX_RESULTS);
    const minLength = Math.max(0, options.minLength ?? 0);
    const cropRegion = options.cropRegion;
    const currentRunId = ++this.runId;

    const run = async () => {
      try {
        const frame = await getFrame();
        if (!frame || currentRunId !== this.runId) {
          return;
        }

        const result = await detectMeterValue(frame, cropRegion);
        if (
          currentRunId !== this.runId ||
          (!result.text && !result.wholePart) ||
          (minLength > 0 && result.wholePart.length < minLength)
        ) {
          return;
        }

        onResult(this.addResult(result));
      } catch (error) {
        console.error("[MeterDetection] Ошибка распознавания кадра:", error);
      } finally {
        if (currentRunId === this.runId) {
          this.timer = setTimeout(execute, intervalMs);
        }
      }
    };

    const execute = () => {
      const task = run();
      this.inFlight = task;
      void task.finally(() => {
        if (this.inFlight === task) {
          this.inFlight = null;
        }
      });
    };

    execute();
  }

  /**
   * Добавление результата в общий live-консенсус.
   */
  addResult(result: MeterRecognitionResult): StableMeterRecognitionResult {
    this.results.push({
      ...result,
      modelConfidence: result.confidence,
      timestamp: Date.now(),
    });
    if (this.results.length > this.maxResults) {
      this.results.shift();
    }

    return buildStableResult(this.results);
  }

  stop(clearResults = true): void {
    this.runId++;
    if (this.timer) {
      clearTimeout(this.timer);
      this.timer = null;
    }
    if (clearResults) {
      this.results = [];
    }
  }

  /**
   * Остановка цикла с ожиданием уже начатого захвата кадра.
   */
  async stopAndWait(clearResults = true): Promise<void> {
    this.stop(clearResults);
    await this.inFlight;
  }
}

function buildStableResult(
  results: MeterDetectionSample[],
): StableMeterRecognitionResult {
  const resultByText = new Map<string, MeterDetectionSample[]>();
  for (const result of results) {
    const matchingResults = resultByText.get(result.text) ?? [];
    matchingResults.push(result);
    resultByText.set(result.text, matchingResults);
  }

  const matchingResults = [...resultByText.values()].sort(
    (first, second) => second.length - first.length,
  )[0];
  const best = matchingResults[matchingResults.length - 1];
  const stability =
    results.length > 1 ? matchingResults.length / results.length : 1;
  const averageModelConfidence =
    matchingResults.reduce((sum, result) => sum + result.modelConfidence, 0) /
    matchingResults.length;

  return {
    ...best,
    confidence: Math.round(averageModelConfidence * stability),
    allResults: results.slice(),
    isReliable: matchingResults.length >= 2,
  };
}

function stripDataUrlPrefix(image: string): string {
  return image.replace(/^data:image\/[a-z0-9.+-]+;base64,/i, "");
}
