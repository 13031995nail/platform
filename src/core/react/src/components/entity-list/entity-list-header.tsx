import type { IEntity } from '@tmobis/core/models/entity';
import { TableHead, TableHeader, TableRow } from '../ui/table';
import { useEntityListContext } from './entity-list-context';
import { getSortingIndicator } from './sorting';

export default function EntityListHeader<TEntity extends IEntity>() {
  const {
    columns,
    hasSelection,
    hasRowActions,
    listData: {
      sorting: { value: sorting, toggle: toggleSorting },
    },
  } = useEntityListContext<TEntity>();

  return (
    <TableHeader>
      <TableRow>
        {hasSelection && <TableHead key="chk" />}
        {columns.map((column, index) => (
          <TableHead
            key={index}
            onPress={column.column
              ? () => toggleSorting(column.column as string)
              : undefined}
          >
            {column.title}
            {column.column && getSortingIndicator(sorting, column.column)}
          </TableHead>
        ))}
        {hasRowActions && <TableHead />}
      </TableRow>
    </TableHeader>
  );
}