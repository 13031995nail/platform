import EntityList from "../entity-list/index";
import type { EntityListProps } from "../entity-list/index";
import { useRouter } from "expo-router";
import type { IEntity } from "../../models/entity";

interface EntityListFormProps<TEntity extends IEntity> extends EntityListProps<TEntity> {
  /**
   * Адрес формы редактирования сущности.
   */
  editRoute: string;
}

export default function EntityListForm<TEntity extends IEntity>(
  props: EntityListFormProps<TEntity>
) {
  const { editRoute } = props;
  const router = useRouter();
  const onRowClick = (rowData?: TEntity) => {
    if (rowData) {
      router.navigate(`/${editRoute}/${rowData.id}`);
    }
  };
  const onAdd = () => {
    router.navigate(`/${editRoute}/new`);
  };

  return (
    <EntityList
      {...props}
      onRowClick={props.onRowClick ?? onRowClick}
      onAdd={props.onAdd ?? onAdd}
    />
  );
}
