import {
  getMessageMetadata,
  InferMessageResult,
  IMessage,
  MessageMetadata,
  MessageType,
} from '../models/message';

export interface MessageProducer {
  messageTypes: readonly MessageType[];
  canSend(message: IMessage, metadata: MessageMetadata): boolean;
  send(message: IMessage, metadata: MessageMetadata, signal?: AbortSignal): Promise<unknown>;
}

/** Направляет сообщения через первого подходящего производителя или резервный канал. */
export class MessageDispatcher {
  private readonly producersByMessageType = new Map<
    MessageType,
    MessageProducer[]
  >();

  constructor(
    producers: readonly MessageProducer[],
    private readonly defaultProducer: MessageProducer
  ) {
    for (const producer of producers) {
      this.registerProducer(producer);
    }
  }

  /** Регистрирует производителя сообщений и возвращает функцию удаления. */
  registerProducer(producer: MessageProducer): () => void {
    for (const messageType of producer.messageTypes) {
      const bucket = this.producersByMessageType.get(messageType);
      if (bucket) {
        bucket.push(producer);
      } else {
        this.producersByMessageType.set(messageType, [producer]);
      }
    }

    let isRegistered = true;
    return () => {
      if (!isRegistered) return;
      isRegistered = false;

      for (const messageType of producer.messageTypes) {
        const bucket = this.producersByMessageType.get(messageType);
        if (!bucket) continue;

        const producerIndex = bucket.indexOf(producer);
        if (producerIndex >= 0) bucket.splice(producerIndex, 1);
        if (bucket.length === 0) {
          this.producersByMessageType.delete(messageType);
        }
      }
    };
  }

  async send<
    TMessage extends IMessage,
    TMessageResult = InferMessageResult<TMessage>
  >(message: TMessage, fallbackProducer?: MessageProducer, signal?: AbortSignal): Promise<TMessageResult> {
    const metadata = getMessageMetadata(message);
    const producers = this.producersByMessageType.get(metadata.type)
      ?? [];
    const producer = producers.find((candidate) =>
      candidate.canSend(message, metadata)
    ) ?? fallbackProducer ?? this.defaultProducer;

    const result = await producer.send(message, metadata, signal);
    return result as TMessageResult;
  }
}
