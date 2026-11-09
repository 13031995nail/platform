import type { IRolesManager } from './roles-manager';

/**
 * Текущий менеджер ролей.
 */
let currentRolesManager: IRolesManager | undefined;

export function setRolesManager(manager: IRolesManager | undefined) {
  currentRolesManager = manager;
}

export const DemoRole : string = 'admin';

export function getCurrentUserRoles(): string[] {
  return currentRolesManager?.roles ?? [];
}

/**
 * Проверяет, имеет ли пользователь хотя бы одну из требуемых ролей.
 * @param requiredRoles - Список ролей, необходимых для доступа.
 * @returns true, если у пользователя есть хотя бы одна из требуемых ролей.
 */
export function hasRequiredRole(requiredRoles?: string[]): boolean {
  if (!requiredRoles || requiredRoles.length === 0) {
    return true;
  }

  const userRoles = getCurrentUserRoles();
  return userRoles.some(role => requiredRoles.includes(role));
}

/**
 * Фильтрует роуты по наличию требуемой роли.
 * @param routes - Список роутов с опциональным свойством roles.
 * @returns Отфильтрованный список роутов, доступных текущему пользователю.
 */
export function filterRoutesByRoles<T extends { roles?: string[] }>(routes: T[]): T[] {
  return routes.filter(route => hasRequiredRole(route.roles));
}
