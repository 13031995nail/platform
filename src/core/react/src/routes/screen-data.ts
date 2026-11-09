export default interface ScreenData {
  /**
   * Путь роута (соответствует имени файла в /routes).
   */
  route: string;

  /**
   * Отображаемое имя роута.
   */
  label: string;

  /**
   * Группировка роута.
   */
  group?: string;

  /**
   * Функция для отрисовки иконки.
   */
  icon?: (props: {
    color: string;
    size: number;
    focused: boolean;
  }) => React.ReactNode;

  /**
   * Роли, которым доступен роут.
   */
  roles?: string[];

  /**
   * Показывать в нижней панели табов на апк (до лимита primary-табов).
   * Остальные экраны попадают во вкладку "Ещё"/ полный drawer на вебе.
   */
  primary?: boolean;

  /**
   * Порядок primary-таба (меньше — левее). Игнорируется без primary.
   */
  priority?: number;

  /**
   * Не показывать в drawer / табах / "Ещё" (служебные роуты).
   */
  hideInNav?: boolean;
}
