/**
 * Константы ролей пользователей.
 */
export const UserRoles = Object.freeze({
  ADMIN: 'admin',
  MANAGER: 'manager',
  USER: 'Пользователь',
  ADMINISTRATOR: 'Администратор',
});

export type UserRole = keyof typeof UserRoles;

/**
 * Интерфейс для сервиса управления ролями.
 * Предоставляет методы для проверки ролей текущего пользователя.
 */
export interface IRolesManager {
  /** Массив ролей текущего пользователя */
  readonly roles: string[];
  /** Проверка наличия роли admin */
  readonly isAdmin: boolean;
  /** Проверка наличия роли manager */
  readonly isManager: boolean;
  /** Проверка наличия роли USER ('Пользователь') */
  readonly isUser: boolean;
  /** Проверка наличия роли ADMINISTRATOR ('Администратор') */
  readonly isAdministrator: boolean;

  /**
   * Проверяет, имеет ли пользователь указанную роль.
   * @param role - Роль из перечисления UserRoles.
   * @returns true, если роль есть у пользователя.
   */
  hasRole(role: UserRole | string): boolean;

  /**
   * Проверяет, имеет ли пользователь хотя бы одну из указанных ролей.
   * @param roles - Массив ролей для проверки.
   * @returns true, если у пользователя есть хотя бы одна из переданных ролей.
   */
  hasAnyRoles(roles: UserRole[] | string[]): boolean;

  /**
   * Проверяет, имеют ли все указанные роли у пользователя.
   * @param roles - Массив ролей для проверки.
   * @returns true, если все роли есть у пользователя.
   */
  hasAllRoles(roles: UserRole[] | string[]): boolean;
}

/**
 * Общая реализация IRolesManager.
 * Получает роли из строки (через запятую) и предоставляет методы для проверки.
 */
export class RolesManager implements IRolesManager {
  public readonly roles: string[];

  constructor(roles: string[] = []) {
    this.roles = roles;
  }

  /**
   * Создает менеджер из строкового представления ролей (через запятую).
   */
  static fromString(rolesString: string): RolesManager {
    const unitedRoles = (rolesString || '').trim();
    const roles = unitedRoles
      ? unitedRoles.split(',').map((r) => r.trim())
      : [];
    return new RolesManager(roles);
  }

  /**
   * Проверяет, имеет ли пользователь указанную роль.
   */
  hasRole(role: UserRole | string): boolean {
    return this.roles.includes(role);
  }

  /**
   * Проверяет, имеет ли пользователь хотя бы одну из указанных ролей.
   */
  hasAnyRoles(roles: UserRole[] | string[]): boolean {
    return roles.some((role) => this.hasRole(role));
  }

  /**
   * Проверяет, имеют ли все указанные роли у пользователя.
   */
  hasAllRoles(roles: UserRole[] | string[]): boolean {
    return roles.every((role) => this.hasRole(role));
  }

  // Геттеры для удобства
  get isAdmin(): boolean {
    return this.hasRole(UserRoles.ADMIN);
  }

  get isManager(): boolean {
    return this.hasRole(UserRoles.MANAGER);
  }

  get isUser(): boolean {
    return this.hasRole(UserRoles.USER);
  }

  get isAdministrator(): boolean {
    return this.hasRole(UserRoles.ADMINISTRATOR);
  }
}
