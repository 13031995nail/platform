import { TmobisSimpleCheckbox } from '@tmobis/core/components/checkbox';
import type { IEntity } from '@tmobis/core/models/entity';
import { TableBody, TableData, TableRow } from '../ui/table';
import { useEntityListContext } from './entity-list-context';
import {
  EntityListRowMenu,
  getColumnContent,
  getVisibleRowButtons,
} from './entity-list-row';

export default function EntityListBody<TEntity extends IEntity>() {
  const {
    columns,
    rowButtons,
    hasRowActions,
    hasSelection,
    isEntitySelected,
    setEntitySelected,
    onRowClick,
    resolveRowConfig,
    listData: { data },
  } = useEntityListContext<TEntity>();

  return (
    <TableBody>
      {data?.map((entity, index) => {
        const rowConfig = resolveRowConfig(entity);
        const checkboxKey = `${index}_chk`;
        const visibleButtons = getVisibleRowButtons(rowButtons, rowConfig, entity);

        return (
          <TableRow
            key={index}
            data={entity}
            className={rowConfig.className}
          >
            {hasSelection && (
              <TableData key={checkboxKey}>
                <TmobisSimpleCheckbox
                  value={entity.id ?? checkboxKey}
                  isChecked={isEntitySelected(entity)}
                  isDisabled={!entity.id || rowConfig.canBeSelected === false}
                  onChange={(isSelected: boolean) =>
                    setEntitySelected(entity, isSelected)
                  }
                />
              </TableData>
            )}
            {columns.map((column, columnIndex) => (
              <TableData
                data={entity}
                onPress={onRowClick}
                key={`${index}_${columnIndex}`}
              >
                {getColumnContent(column, entity)}
              </TableData>
            ))}
            {hasRowActions && (
              <TableData>
                {visibleButtons.length > 0 && (
                  <EntityListRowMenu entity={entity} buttons={visibleButtons} />
                )}
              </TableData>
            )}
          </TableRow>
        );
      })}
    </TableBody>
  );
}
