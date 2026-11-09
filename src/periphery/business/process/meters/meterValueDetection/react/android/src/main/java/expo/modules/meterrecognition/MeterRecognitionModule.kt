package expo.modules.meterrecognition

import expo.modules.kotlin.modules.Module
import expo.modules.kotlin.modules.ModuleDefinition

class MeterRecognitionModule : Module() {
  private val recognitionLock = Any()
  private var recognizer: MeterRecognizer? = null

  override fun definition() = ModuleDefinition {
    Name("MeterRecognition")

    AsyncFunction("detectDigits") {
        base64Image: String,
        cropLeft: Double?,
        cropTop: Double?,
        cropWidth: Double?,
        cropHeight: Double? ->
      synchronized(recognitionLock) {
        val context = appContext.reactContext?.applicationContext
          ?: throw IllegalStateException("Android context is unavailable")
        val detector = recognizer ?: MeterRecognizer(context).also {
          recognizer = it
        }

        detector.detectDigits(
          base64Image,
          cropLeft?.toFloat(),
          cropTop?.toFloat(),
          cropWidth?.toFloat(),
          cropHeight?.toFloat()
        )
      }
    }

    OnDestroy {
      synchronized(recognitionLock) {
        recognizer?.close()
        recognizer = null
      }
    }
  }
}
