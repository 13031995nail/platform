import { FlashList } from '@shopify/flash-list';
import { useCallback } from 'react';
import { StyleSheet, View } from 'react-native';
import type { IEntity } from '@tmobis/core/models/entity';
import { useEntityListContext } from './entity-list-context';
import EntityListPagination from './pagination';
import EntityListBody from './entity-list-body.native';

type Props = { headerSlot?: React.ReactNode };

export default function EntityListView<TEntity extends IEntity>({ headerSlot }: Props) {
  const { listData: { data, paging } } = useEntityListContext<TEntity>();

  const renderItem = useCallback(({ item, index }: { item: TEntity; index: number }) => (
    <View style={styles.rowWrapper}>
      <EntityListBody entity={item} index={index} />
    </View>
  ), []);


  return (
    <FlashList
      data={data ?? []}
      renderItem={renderItem}
      keyExtractor={(item: TEntity, i: number) => String(item.id ?? i)}
      ListHeaderComponent={<>{headerSlot}</>}
      ListFooterComponent={<EntityListPagination paging={paging} />}
      maintainVisibleContentPosition={{ disabled: true }}
      contentContainerStyle={styles.content}
    />
  );
}

const styles = StyleSheet.create({
  content: {
    paddingBottom: 24
  },
  rowWrapper: {
    paddingBottom: 12
  },
});