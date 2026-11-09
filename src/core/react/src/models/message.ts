export type MessageType = 'query' | 'event' | 'command';

export interface MessageMetadata {
  type: MessageType;
  group?: string;
  name: string;
}

/**
 * Базовый контракт сообщения.
 *
 * @example
 * ```ts
 * interface GetUserResult {
 *   id: string;
 *   name: string;
 * }
 *
 * class GetUser extends Message<GetUserResult> {
 *   static metadata = {
 *     type: 'query',
 *     group: 'users',
 *     name: 'GetUser',
 *   } satisfies MessageMetadata;
 *
 *   constructor(public id: string) {}
 * }
 * ```
 */
export interface IMessage<TMessageResult = unknown> {
  context: Record<string, unknown>;
  
  /** Только для вывода типа */
  readonly __resultType?: TMessageResult;
}

export abstract class Message<TMessageResult = unknown>
  implements IMessage<TMessageResult>
{
  context: Record<string, unknown> = {};

  readonly __resultType!: TMessageResult;
}

/** Извлекает тип ответа из generic-параметра Message. */
export type InferMessageResult<TMessage> =
  TMessage extends IMessage<infer TMessageResult> ? TMessageResult : never;

interface MessageConstructor {
  name: string;
  metadata?: MessageMetadata;
}

export const getMessageMetadata = (message: IMessage): MessageMetadata => {
  const constructor = message.constructor as MessageConstructor;
  const metadata = constructor.metadata;

  if (!metadata) {
    throw new Error(
      `Сообщение "${constructor.name}" должно содержать статические метаданные { type, group?, name }.`
    );
  }

  if (!['query', 'event', 'command'].includes(metadata.type)
    || !metadata.name) {
    throw new Error(
      `Сообщение "${constructor.name}" содержит некорректные статические метаданные { type, group?, name }.`
    );
  }

  return metadata;
};
