import assert from "node:assert/strict";
import { test } from "node:test";
import {
  defineListFormType,
  resolveListFormColumns,
} from "./list-form-extension";
import type {
  ListFormExtension,
  ListFormExtensionContext,
} from "./list-form-extension";

const dictionaryListFormType = defineListFormType("dictionary");

test("resolveListFormColumns removes duplicate named columns while preserving order", () => {
  const base = [
    { column: "name", title: "Name" },
    { column: "shared", title: "Base" },
    { column: "shared", title: "Duplicate base" },
  ];
  const extensions: ListFormExtension[] = [
    {
      listFormType: dictionaryListFormType,
      columns: [
        { column: "shared", title: "First" },
        { column: "code", title: "Code" },
      ],
    },
    {
      listFormType: dictionaryListFormType,
      columns: [
        { column: "code", title: "Second" },
      ],
    },
  ];

  const result = resolveListFormColumns(base, extensions, {
    listFormType: dictionaryListFormType,
    entityType: "Dictionary",
  });

  assert.deepEqual(result.map(({ column }) => column), ["name", "shared", "code"]);
  assert.notStrictEqual(result, base);
  assert.equal(base.length, 3);
  assert.equal(base[2].title, "Duplicate base");
  assert.strictEqual(result[1], base[1]);
});

test("resolveListFormColumns passes complete context to dynamic columns", () => {
  const context: ListFormExtensionContext = {
    listFormType: dictionaryListFormType,
    entityType: "Dictionary",
    viewName: "DictionaryL",
    editRoute: "dictionaries",
  };
  let receivedContext: ListFormExtensionContext | undefined;

  const result = resolveListFormColumns([], [{
    listFormType: dictionaryListFormType,
    columns: value => {
      receivedContext = value;
      return [{ column: "actuality" }];
    },
  }], context);

  assert.strictEqual(receivedContext, context);
  assert.deepEqual(result, [{ column: "actuality" }]);
});
