import assert from "node:assert/strict";
import { test } from "node:test";
import { CompositionRoot } from "./composition-root";
import { defineListFormType } from "./list-form-extension";
import type { ListFormExtension } from "./list-form-extension";
import ModuleBase from "./module";
import { createApiClient } from '../api/create-api-client';

test("CompositionRoot collects list form extensions without executing factories", () => {
  const dictionaryListFormType = defineListFormType("dictionary");
  const otherListFormType = defineListFormType("other");
  let factoryCalls = 0;
  const extension: ListFormExtension = {
    listFormType: dictionaryListFormType,
    columns: () => {
      factoryCalls += 1;
      return [{ column: "actuality" }];
    },
  };
  class TestModule extends ModuleBase {
    getListFormExtensions() {
      return [extension];
    }
  }

  const root = new CompositionRoot([new TestModule()]);

  assert.deepEqual(root.getListFormExtensions(dictionaryListFormType), [extension]);
  assert.deepEqual(root.getListFormExtensions(otherListFormType), []);
  assert.equal(factoryCalls, 0);
});

test("CompositionRoot использует один и тот же клиент во всех модулях", () => {
  const client = createApiClient({ baseURL: 'https://backend.test' });
  let injected: unknown;
  class TestModule extends ModuleBase {
    configureApiClient(value: typeof client) { injected = value; }
  }
  const root = new CompositionRoot([new TestModule()], client);
  assert.strictEqual(root.getApiClient(), client);
  assert.strictEqual(injected, client);
});
