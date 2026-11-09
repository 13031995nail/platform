import assert from 'node:assert/strict';
import { test } from 'node:test';
import { mergeSelectedEntities, notifySingleLookupChange } from './selection';

type Entity = { id: string; name: string };

const alice: Entity = { id: 'a', name: 'Алиса' };
const bob: Entity = { id: 'b', name: 'Борис' };

test('multi lookup сохраняет выбор между серверными поисковыми выдачами', () => {
	const selected = mergeSelectedEntities([alice], ['a', 'b'], [bob]);
	assert.deepEqual(selected, [alice, bob]);
});

test('multi lookup удаляет снятые отметки из прежних поисковых выдач', () => {
	const selected = mergeSelectedEntities([alice, bob], ['b'], [alice]);
	assert.deepEqual(selected, [bob]);
});

test('multi lookup использует актуальную сущность из новой выдачи', () => {
	const updatedAlice: Entity = { id: 'a', name: 'Алиса (обновлена)' };
	const selected = mergeSelectedEntities([alice], ['a'], [updatedAlice]);
	assert.deepEqual(selected, [updatedAlice]);
});

test('single lookup вызывает один обработчик, если переданы оба callback', () => {
	const calls: string[] = [];
	notifySingleLookupChange('a', alice, {
		onChange: (entity) => calls.push(`entity:${entity?.id}`),
		onChangeId: (id) => calls.push(`id:${id}`),
		onFieldChange: (id) => calls.push(`field:${id}`),
	});
	assert.deepEqual(calls, ['entity:a']);
});

test('single lookup выбирает callback ID или поле, если нет callback сущности', () => {
	const ids: Array<string | undefined> = [];
	const fields: Array<string | null> = [];
	notifySingleLookupChange('a', alice, {
		onChangeId: (id) => ids.push(id),
		onFieldChange: (id) => fields.push(id),
	});
	notifySingleLookupChange(null, null, {
		onFieldChange: (id) => fields.push(id),
	});
	assert.deepEqual(ids, ['a']);
	assert.deepEqual(fields, [null]);
});
