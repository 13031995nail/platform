import { useTheme } from '@react-navigation/native';
import React from 'react';
import { StyleSheet, Text, View } from 'react-native';
import { NotificationActionButton } from './notification-action-button';

interface NotificationPaginationProps {
  onNext: () => void;
  onPrevious: () => void;
  page: number;
  pageCount: number;
  pageSize: number;
  total: number;
}

export function NotificationPagination({
  onNext,
  onPrevious,
  page,
  pageCount,
  pageSize,
  total,
}: NotificationPaginationProps) {
  const { colors } = useTheme();

  if (total <= pageSize) {
    return null;
  }

  return (
    <View style={[styles.paging, { borderColor: colors.border }]}>
      <NotificationActionButton
        disabled={page === 1}
        onPress={onPrevious}
      >
        Назад
      </NotificationActionButton>
      <Text style={{ color: colors.text }}>
        {page} из {pageCount}
      </Text>
      <NotificationActionButton
        disabled={page >= pageCount}
        onPress={onNext}
      >
        Далее
      </NotificationActionButton>
    </View>
  );
}

const styles = StyleSheet.create({
  paging: {
    alignItems: 'center',
    borderTopWidth: 1,
    flexDirection: 'row',
    justifyContent: 'space-between',
    padding: 12,
  },
});
