import type { IMessage, MessageMetadata } from '../models/message';
import type { MessageProducer } from '../messages/dispatcher';
import type { MessageApi } from './services/message-api';

/** HTTP-адаптер с внедрённым сервисом: диспетчер не зависит от Axios. */
export function createWebApiMessageProducer(
  service: MessageApi
): MessageProducer {
  return {
    messageTypes: ['query', 'event', 'command'],
    canSend(_message: IMessage, _metadata: MessageMetadata): boolean {
      return true;
    },
    send(message: IMessage, metadata: MessageMetadata, signal?: AbortSignal): Promise<unknown> {
      return service.sendMessage(message, metadata, signal);
    },
  };
}
