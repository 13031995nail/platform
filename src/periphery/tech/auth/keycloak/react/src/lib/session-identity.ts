import type { StoredTokens } from './types';

/**
 * Идентификация пользователя и области его прав.
 * Обновление access-токена без изменения идентичности/прав не сбрасывает кеш.
 */
export function resolveSessionIdentity(tokens: Readonly<StoredTokens> | null): string | null {
  if (!tokens) return null;
  const backend = tokens.backendAccessToken;
  const principal = [backend.user, backend.employee, backend.userName].filter(Boolean);

  // Без устойчивого идентификатора обновление токена сбрасывает кеш (fail closed).
  if (!principal.length) {
    return backend.accessToken;
  }

  return JSON.stringify([principal, backend.town ?? null, backend.roles ?? null]);
}
