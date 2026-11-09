import { useFocusEffect, useTheme } from '@react-navigation/native';
import { useHeader } from '@tmobis/core/components/header/context';
import React, { useCallback } from 'react';
import {
  ActivityIndicator,
  FlatList,
  StyleSheet,
  Text,
  View,
} from 'react-native';
import {
  NOTIFICATIONS_PAGE_SIZE,
} from '../../messages/get-notifications';
import { NotificationActionButton } from './notification-action-button';
import { NotificationListItem } from './notification-list-item';
import { NotificationPagination } from './notification-pagination';
import { useNotifications } from './use-notifications';

export default function NotificationsScreen() {
  const { colors } = useTheme();
  const { setHeaderData } = useHeader();
  const {
    goToNextPage,
    goToPreviousPage,
    notificationsQuery,
    page,
    pageCount,
    total,
  } = useNotifications(NOTIFICATIONS_PAGE_SIZE);

  useFocusEffect(
    useCallback(() => {
      setHeaderData({ title: 'Уведомления' });
    }, [setHeaderData])
  );

  if (notificationsQuery.isLoading) {
    return (
      <View style={styles.center}>
        <ActivityIndicator color={colors.primary} />
      </View>
    );
  }

  if (notificationsQuery.isError) {
    return (
      <View style={styles.center}>
        <Text style={{ color: colors.text }}>
          Не удалось загрузить уведомления
        </Text>
        <NotificationActionButton
          onPress={() => void notificationsQuery.refetch()}
        >
          Повторить
        </NotificationActionButton>
      </View>
    );
  }

  return (
    <View style={[styles.container, { backgroundColor: colors.background }]}>
      <FlatList
        contentContainerStyle={styles.list}
        data={notificationsQuery.data?.data ?? []}
        keyExtractor={(item, index) => item.id ?? index.toString()}
        renderItem={({ item }) => (
          <NotificationListItem notification={item} />
        )}
        ListEmptyComponent={(
          <Text style={[styles.empty, { color: colors.text }]}>
            Уведомлений пока нет
          </Text>
        )}
      />
      <NotificationPagination
        onNext={goToNextPage}
        onPrevious={goToPreviousPage}
        page={page}
        pageCount={pageCount}
        pageSize={NOTIFICATIONS_PAGE_SIZE}
        total={total}
      />
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
  },
  list: {
    flexGrow: 1,
    gap: 8,
    padding: 12,
  },
  empty: {
    marginTop: 32,
    textAlign: 'center',
  },
  center: {
    alignItems: 'center',
    flex: 1,
    gap: 12,
    justifyContent: 'center',
  },
});
