import {
  type IMessage,
  type MessageMetadata,
} from '@tmobis/core/models/message';
import type {MessageProducer} from '@tmobis/core/messages/dispatcher';
import SignalRClient, {type SignalROutgoingMessage} from './client';

/** Направляет сообщения-события через SignalR. */
export function createSignalRMessageProducer(
  client: SignalRClient
): MessageProducer {
  return {
    messageTypes: ['event'],
    canSend(message: IMessage, metadata): boolean {
      return client.canSend(message, metadata);
    },
    send(
      message: IMessage,
      metadata: MessageMetadata
    ): Promise<void> {
      return client.send(message as SignalROutgoingMessage, metadata);
    },
  };
}
