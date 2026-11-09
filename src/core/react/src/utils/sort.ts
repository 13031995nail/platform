import { DateOnly, getTimestamp } from './date';

type WithRenameDate<T> = T & { renameDate?: DateOnly | Date | string | null };

export const sortByNameHistory = <T>(a: WithRenameDate<T>, b: WithRenameDate<T>) => {
  const renameDateA = getTimestamp(a.renameDate, true);
  const renameDateB = getTimestamp(b.renameDate, true);

  return renameDateB - renameDateA;
};
