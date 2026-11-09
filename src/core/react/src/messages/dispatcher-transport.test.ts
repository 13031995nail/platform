import assert from 'node:assert/strict';
import { test } from 'node:test';
import { Message, type MessageMetadata } from '../models/message';
import { MessageDispatcher, type MessageProducer } from './dispatcher';

class TestCommand extends Message<string> {
  static metadata = { type: 'command', name: 'TestCommand' } satisfies MessageMetadata;
}

function producer(value: string, matches: boolean): MessageProducer {
  return {
    messageTypes: ['command'],
    canSend: () => matches,
    send: async () => value,
  };
}

test('Пользовательский producer приоритетнее резервного', async () => {
  const dispatcher = new MessageDispatcher([producer('custom', true)], producer('global', true));
  assert.equal(await dispatcher.send(new TestCommand(), producer('scoped', true)), 'custom');
});

test('Резервный producer выбирается без зависимости от HTTP-клиента', async () => {
  const dispatcher = new MessageDispatcher([producer('unused', false)], producer('global', true));
  assert.equal(await dispatcher.send(new TestCommand(), producer('scoped', true)), 'scoped');
  assert.equal(await dispatcher.send(new TestCommand()), 'global');
});

test('Диспетчер передаёт сигнал отмены без зависимости от Axios', async () => {
  const controller = new AbortController();
  let seenSignal: AbortSignal | undefined;
  const custom: MessageProducer = {
    messageTypes: ['command'],
    canSend: () => true,
    send: async (_message, _metadata, signal) => {
      seenSignal = signal;
      return 'ok';
    },
  };
  const dispatcher = new MessageDispatcher([custom], producer('fallback', true));
  assert.equal(await dispatcher.send(new TestCommand(), undefined, controller.signal), 'ok');
  assert.strictEqual(seenSignal, controller.signal);
});
