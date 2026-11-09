import type { EntityListViewColumn } from "../components/entity-list";
import type { IEntity } from "../models/entity";

export interface ListFormExtensionContext {
  readonly listFormType: ListFormType;
  readonly entityType: string;
  readonly viewName?: string;
  readonly editRoute?: string;
}

declare const listFormTypeBrand: unique symbol;

export type ListFormType = string & {
  readonly [listFormTypeBrand]: "ListFormType";
};

export function defineListFormType(value: string): ListFormType {
  return value as ListFormType;
}

export type ListFormColumn<TEntity extends IEntity = IEntity> =
  EntityListViewColumn<TEntity>;

export type ListFormExtensionColumns =
  | readonly ListFormColumn[]
  | ((context: ListFormExtensionContext) => readonly ListFormColumn[]);

export interface ListFormExtension {
  readonly listFormType: ListFormType;
  readonly columns?: ListFormExtensionColumns;
}

function appendUniqueColumns<TEntity extends IEntity>(
  target: EntityListViewColumn<TEntity>[],
  source: readonly EntityListViewColumn<TEntity>[],
  knownColumnNames: Set<string>
): void {
  for (const column of source) {
    const columnName = column.column;
    if (columnName !== undefined && knownColumnNames.has(columnName)) {
      continue;
    }

    target.push(column);
    if (columnName !== undefined) {
      knownColumnNames.add(columnName);
    }
  }
}

export function resolveListFormColumns<TEntity extends IEntity>(
  baseColumns: readonly EntityListViewColumn<TEntity>[],
  extensions: readonly ListFormExtension[],
  context: ListFormExtensionContext
): EntityListViewColumn<TEntity>[] {
  const knownColumnNames = new Set<string>();
  const resolvedColumns: EntityListViewColumn<TEntity>[] = [];
  appendUniqueColumns(resolvedColumns, baseColumns, knownColumnNames);

  for (const extension of extensions) {
    const extensionColumns = typeof extension.columns === "function"
      ? extension.columns(context)
      : extension.columns;

    if (extensionColumns) {
      appendUniqueColumns(
        resolvedColumns,
        extensionColumns,
        knownColumnNames
      );
    }
  }

  return resolvedColumns;
}
