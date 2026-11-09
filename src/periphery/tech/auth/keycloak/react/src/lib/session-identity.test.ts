import assert from 'node:assert/strict';
import { test } from 'node:test';
import type { StoredTokens } from './types';
import { resolveSessionIdentity } from './session-identity';

function makeTokens(user: string, accessToken: string, roles = 'user'): StoredTokens {
  return {
    accessToken: 'keycloak-access',
    idToken: 'keycloak-id',
    backendAccessToken: { accessToken, user, roles, town: 'A' },
  };
}

test('Обновление access-токена того же пользователя не меняет идентичность сеанса', () => {
  assert.equal(
    resolveSessionIdentity(makeTokens('123', 'old')),
    resolveSessionIdentity(makeTokens('123', 'new'))
  );
});

test('Logout, смена пользователя или прав меняют идентичность сеанса', () => {
  const first = resolveSessionIdentity(makeTokens('123', 'old'));
  assert.notEqual(first, resolveSessionIdentity(null));
  assert.notEqual(first, resolveSessionIdentity(makeTokens('456', 'new')));
  assert.notEqual(first, resolveSessionIdentity(makeTokens('123', 'new', 'admin')));
});

test('При отсутствии идентификатора смена токена сбрасывает кеш', () => {
  assert.notEqual(
    resolveSessionIdentity(makeTokens('', 'token-1')),
    resolveSessionIdentity(makeTokens('', 'token-2'))
  );
});
