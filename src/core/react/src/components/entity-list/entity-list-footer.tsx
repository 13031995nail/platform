import { TableCaption } from '../ui/table';
import { useEntityListContext } from './entity-list-context';
import EntityListPagination from './pagination';

export default function EntityListFooter() {
  const { paging } = useEntityListContext().listData;

  return (
    <TableCaption>
      <EntityListPagination paging={paging} />
    </TableCaption>
  );
}
