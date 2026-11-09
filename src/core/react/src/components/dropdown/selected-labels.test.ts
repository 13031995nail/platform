import assert from 'node:assert/strict';
import { test } from 'node:test';
import { rememberSelectedLabels } from './selected-labels';

test('выбранная метка сохраняется при изменении серверной выдачи', () => {
	const selected = ['a'];
	const initial = rememberSelectedLabels(new Map(), selected, [
		{ value: 'a', label: 'Москва' },
	]);
	const filtered = rememberSelectedLabels(initial, selected, [
		{ value: 'b', label: 'Томск' },
	]);
	assert.equal(filtered.get('a'), 'Москва');
});

test('возвращение выбранного объекта в выдачу обновляет его подпись', () => {
	const known = new Map([['a', 'Москва']]);
	const refreshed = rememberSelectedLabels(known, ['a'], [
		{ value: 'a', label: 'Город Москва' },
	]);
	assert.equal(refreshed.get('a'), 'Город Москва');
});

test('multi-select сохраняет все выбранные метки и удаляет снятые', () => {
	const previous = new Map([['a', 'Москва'], ['b', 'Томск']]);
	const filtered = rememberSelectedLabels(previous, ['a', 'b'], [
		{ value: 'c', label: 'Омск' },
	]);
	assert.deepEqual(Array.from(filtered.entries()), [
		['a', 'Москва'], ['b', 'Томск'],
	]);
	const removed = rememberSelectedLabels(filtered, ['b'], []);
	assert.deepEqual(Array.from(removed.entries()), [['b', 'Томск']]);
});

test('очистка выбора очищает кэш меток', () => {
	const cleared = rememberSelectedLabels(new Map([['a', 'Москва']]), [], []);
	assert.equal(cleared.size, 0);
});
