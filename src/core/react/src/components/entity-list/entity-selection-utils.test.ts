import assert from "node:assert/strict";
import { test } from "node:test";
import {
	getPageEntitiesByIds,
	getSelectableEntities,
	setEntityIdSelected,
	toggleSelectedIds,
} from "./entity-selection-utils";

test("Выбор по ID сохраняется после обновления объектов", () => {
	const before = { id: "1", name: "старое" };
	const after = { id: "1", name: "новое" };
	const selected = setEntityIdSelected(new Set<string>(), before.id, true);
	assert.ok(selected.has(after.id));
	assert.deepEqual(getPageEntitiesByIds([after], selected), [after]);
});

test("Выбор всех записей не затрагивает предыдущую страницу и переключается повторно", () => {
	const previous = new Set(["old"]);
	const selected = toggleSelectedIds(previous, ["one", "two", "two"]);
	assert.deepEqual([...previous], ["old"]);
	assert.deepEqual([...selected], ["old", "one", "two"]);
	assert.deepEqual([...toggleSelectedIds(selected, ["one", "two"])], ["old"]);
});

test("Строки без ID и с запретом выбора исключаются из select-all", () => {
	const data = [
		{ id: "one", canBeSelected: true },
		{ canBeSelected: true },
		{ id: "two", canBeSelected: false },
	];
	const selectable = getSelectableEntities(data, row => row.canBeSelected);
	assert.deepEqual(selectable.map(row => row.id), ["one"]);
	const selected = toggleSelectedIds(new Set<string>(), selectable.map(row => row.id!));
	assert.equal(selectable.every(row => selected.has(row.id!)), true);
	assert.deepEqual(getPageEntitiesByIds(data, selected), [data[0]]);
});

test("Повторная установка того же выбора не создаёт новый Set", () => {
	const selected = new Set(["one"]);
	assert.strictEqual(setEntityIdSelected(selected, "one", true), selected);
	assert.strictEqual(setEntityIdSelected(selected, "", true), selected);
	assert.strictEqual(toggleSelectedIds(selected, []), selected);
	assert.deepEqual([...setEntityIdSelected(selected, "one", false)], []);
});

test("Большой набор выбранных ID корректно обрабатывает текущую страницу", () => {
	const selected = new Set(Array.from({ length: 10_000 }, (_, i) => `id-${i}`));
	const page = Array.from({ length: 100 }, (_, i) => ({ id: `id-${i + 5_000}` }));
	assert.equal(getPageEntitiesByIds(page, selected).length, 100);
	const after = toggleSelectedIds(selected, page.map(entity => entity.id));
	assert.equal(after.size, 9_900);
	assert.ok(after.has("id-0"));
	assert.ok(!after.has("id-5000"));
});
