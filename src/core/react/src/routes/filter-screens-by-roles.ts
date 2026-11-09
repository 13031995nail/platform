import ScreenData from './screen-data';
import { filterRoutesByRoles } from '../security/permissions';

/**
 * Отбор экранов по ролям текущего пользователя.
 */
export function filterScreensByRoles(screens: ScreenData[]): ScreenData[] {
  return filterRoutesByRoles(screens);
}
