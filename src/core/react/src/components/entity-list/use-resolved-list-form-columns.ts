import { useMemo } from "react";
import type { IEntity } from "../../models/entity";
import { resolveListFormColumns } from "../../modules/list-form-extension";
import type { ListFormType } from "../../modules/list-form-extension";
import { useOptionalCompositionRoot } from "../../modules/provider";
import type { EntityListViewColumn } from "./entity-list-types";

interface UseResolvedListFormColumnsOptions<TEntity extends IEntity> {
  columns: readonly EntityListViewColumn<TEntity>[];
  listFormType?: ListFormType;
  entityType: string;
  viewName?: string;
  editRoute?: string;
}

export default function useResolvedListFormColumns<TEntity extends IEntity>({
  columns,
  listFormType,
  entityType,
  viewName,
  editRoute,
}: UseResolvedListFormColumnsOptions<TEntity>): readonly EntityListViewColumn<TEntity>[] {
  const compositionRoot = useOptionalCompositionRoot();

  return useMemo(() => {
    if (!listFormType || !compositionRoot) {
      return columns;
    }

    return resolveListFormColumns(
      columns,
      compositionRoot.getListFormExtensions(listFormType),
      {
        listFormType,
        entityType,
        viewName,
        editRoute,
      }
    );
  }, [
    columns,
    compositionRoot,
    editRoute,
    entityType,
    listFormType,
    viewName,
  ]);
}
